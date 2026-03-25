using MongoDB.Driver;
using FlightTicketsAPI.Models;
using Microsoft.Extensions.Options;
using Prometheus;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Net.Sockets;

namespace FlightTicketsAPI.Services.Impl
{
    public class FlightTicketService : IFlightTicketService
    {
        private readonly IMongoCollection<EntityFlightTicket> _ticketsCollection;

        private readonly IDistributedCache _cache;

        private const string CacheKeyAllTickets = "tickets:all";

        private readonly TimeSpan _shortTime = TimeSpan.FromMinutes(5);

        private readonly TimeSpan _longTime = TimeSpan.FromMinutes(15);

        private readonly TimeSpan _shortestTime = TimeSpan.FromMinutes(1);

        private static readonly Histogram MongoQueryDuration = Metrics.
            CreateHistogram("mongodb_query_duration_seconds", "Время выполнения запросов к MongoDB",
                new HistogramConfiguration
                {
                    LabelNames = ["operation", "collection"],
                    Buckets = [0.001, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1.0, 2.0]
                }
            );

        private static readonly Counter CacheRequestsTotal = Metrics.CreateCounter(
                "redis_cache_requests_total", "Количество обращений к кэшу",
                new CounterConfiguration { LabelNames = ["operation", "result"] }
            );

        private static readonly Histogram CacheDuration = Metrics.
            CreateHistogram("redis_cache_duration_seconds", "Время выполнения запросов к Redis",
                new HistogramConfiguration
                {
                    LabelNames = ["operation"],
                    Buckets = [0.0005, 0.001, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1]
                }
            );

        public FlightTicketService(IOptions<FlightTicketsDBSettings> flightTicketsDBSettings, IDistributedCache cache)
        {
            var mongoClient = new MongoClient(flightTicketsDBSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(flightTicketsDBSettings.Value.DatabaseName);
            _ticketsCollection = mongoDatabase.GetCollection<EntityFlightTicket>(flightTicketsDBSettings.Value.CollectionName);
            _cache = cache;
        }

        public async Task<List<EntityFlightTicket>> GetAllAsync()
        {
            string cachedData;
            using (CacheDuration.WithLabels("get").NewTimer())
            {
                cachedData = await _cache.GetStringAsync(CacheKeyAllTickets);
            }
            if(!string.IsNullOrEmpty(cachedData))
            {
                CacheRequestsTotal.WithLabels("get", "hit").Inc();
                return JsonSerializer.Deserialize<List<EntityFlightTicket>>(cachedData)!;
            }
            CacheRequestsTotal.WithLabels("get", "miss").Inc();
            List<EntityFlightTicket> tickets;
            using(MongoQueryDuration.WithLabels("get_all", "flight_tickets").NewTimer())
            {
                tickets =  await _ticketsCollection.Aggregate().Sample(1000).ToListAsync();
            }
            var serializedData = JsonSerializer.Serialize(tickets);
            using (CacheDuration.WithLabels("set").NewTimer())
            {
                await _cache.SetStringAsync(
                    CacheKeyAllTickets,
                    serializedData,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _longTime }
                );
            }
            CacheRequestsTotal.WithLabels("set", "success").Inc();
            return tickets;
        }

        public async Task<EntityFlightTicket> GetAsync(string id)
        {
            string cacheKey = $"ticket:{id}";
            string cachedData;
            using (CacheDuration.WithLabels("get").NewTimer())
            {
                cachedData = await _cache.GetStringAsync(cacheKey);
            }
            if (!string.IsNullOrEmpty(cachedData))
            {
                CacheRequestsTotal.WithLabels("get", "hit").Inc();
                return JsonSerializer.Deserialize<EntityFlightTicket>(cachedData)!;
            }
            CacheRequestsTotal.WithLabels("get", "miss").Inc();
            EntityFlightTicket ticket;
            using (MongoQueryDuration.WithLabels("get_by_id", "flight_tickets").NewTimer())
            {
                ticket = await _ticketsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            if(ticket != null)
            {
                var serializedData = JsonSerializer.Serialize(ticket);
                using (CacheDuration.WithLabels("set").NewTimer())
                {
                    await _cache.SetStringAsync(
                        cacheKey,
                        JsonSerializer.Serialize(ticket),
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _shortTime }
                    );
                    CacheRequestsTotal.WithLabels("set", "success").Inc();
                }
            }
            return ticket!;
        }
        
        public async Task CreateAsync(EntityFlightTicket entityFlightTicket)
        {
            using (MongoQueryDuration.WithLabels("insert", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.InsertOneAsync(entityFlightTicket);
            }
            using (CacheDuration.WithLabels("remove").NewTimer())
            {
                await _cache.RemoveAsync(CacheKeyAllTickets);
            }
            CacheRequestsTotal.WithLabels("remove", "success").Inc();
        }
            
        public async Task UpdateAsync(string id, EntityFlightTicket updatedEntityFlightTicket)
        {
            using (MongoQueryDuration.WithLabels("update", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.ReplaceOneAsync(x => x.Id == id, updatedEntityFlightTicket);
            }
            using (CacheDuration.WithLabels("remove").NewTimer())
            {
                await _cache.RemoveAsync($"ticket:{id}");
            }
            using (CacheDuration.WithLabels("remove").NewTimer())
            {
                await _cache.RemoveAsync(CacheKeyAllTickets);
            }
            CacheRequestsTotal.WithLabels("remove", "success").Inc(2);
        }
            
        public async Task DeleteAsync(string id)
        {
            using (MongoQueryDuration.WithLabels("delete", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.DeleteOneAsync(x => x.Id == id);
            }
            using (CacheDuration.WithLabels("remove").NewTimer())
            {
                await _cache.RemoveAsync($"ticket:{id}");
            }
            using (CacheDuration.WithLabels("remove").NewTimer())
            {
                await _cache.RemoveAsync(CacheKeyAllTickets);
            }
            CacheRequestsTotal.WithLabels("remove", "success").Inc(2);
        } 
            
        public async Task ClearAllAsync()
        {
            using (MongoQueryDuration.WithLabels("delete_all", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.DeleteManyAsync(_ => true);
            }
            using (CacheDuration.WithLabels("remove").NewTimer())
            {
                await _cache.RemoveAsync(CacheKeyAllTickets);
            }
            CacheRequestsTotal.WithLabels("remove", "success").Inc();
        }
        public async Task<List<EntityFlightTicket>> GetFilteredAsync(FlightTicketFilter ftl)
        {
            string cacheKey = GenerateFilterCacheKey(ftl);
            string cachedData;
            using (CacheDuration.WithLabels("get").NewTimer())
            {
                cachedData = await _cache.GetStringAsync(cacheKey);
            }
            if (!string.IsNullOrEmpty(cachedData))
            {
                CacheRequestsTotal.WithLabels("get", "hit").Inc();
                return JsonSerializer.Deserialize<List<EntityFlightTicket>>(cachedData)!;
            }
            CacheRequestsTotal.WithLabels("get", "miss").Inc();
            var builder = Builders<EntityFlightTicket>.Filter;
            var filter = builder.Empty;
            if (!string.IsNullOrWhiteSpace(ftl.FlightNumber))
            {
                filter &= builder.Eq(x => x.FlightNumber, ftl.FlightNumber);
            }
            if (!string.IsNullOrWhiteSpace(ftl.DepartureCode))
            {
                filter &= builder.Eq(x => x.DepartureCode, ftl.DepartureCode);
            }
            if (!string.IsNullOrWhiteSpace(ftl.ArrivalCode))
            {
                filter &= builder.Eq(x => x.ArrivalCode, ftl.ArrivalCode);
            }
            if (ftl.MinPrice.HasValue)
            {
                filter &= builder.Gte(x => x.Price, ftl.MinPrice.Value);
            }
            if (ftl.MaxPrice.HasValue)
            {
                filter &= builder.Lte(x => x.Price, ftl.MaxPrice.Value);
            }
            if (ftl.DepartureDateFrom.HasValue)
            {
                filter &= builder.Gte(x => x.DepartureTime, ftl.DepartureDateFrom.Value);
            }
            if (ftl.DepartureDateTo.HasValue)
            {
                filter &= builder.Lte(x => x.DepartureTime, ftl.DepartureDateTo.Value);
            }
            List<EntityFlightTicket> tickets;
            using (MongoQueryDuration.WithLabels("get_filtered", "flight_tickets").NewTimer())
            {
                tickets = await _ticketsCollection.Find(filter).ToListAsync();
            }
            var serializedData = JsonSerializer.Serialize(tickets);
            using (CacheDuration.WithLabels("set").NewTimer())
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    serializedData,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _shortestTime }
                );
            }
            CacheRequestsTotal.WithLabels("set", "success").Inc();
            return tickets;
        }

        private string GenerateFilterCacheKey(FlightTicketFilter ftl)
        {
            return $"tickets:filter:{ftl.FlightNumber}_{ftl.DepartureCode}_{ftl.ArrivalCode}_{ftl.MinPrice}_{ftl.MaxPrice}_{ftl.DepartureDateFrom?.Ticks}_{ftl.DepartureDateTo?.Ticks}";
        }
    }
}
