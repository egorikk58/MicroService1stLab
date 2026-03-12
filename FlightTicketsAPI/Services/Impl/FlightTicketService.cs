using MongoDB.Driver;
using FlightTicketsAPI.Models;
using Microsoft.Extensions.Options;
using Prometheus;

namespace FlightTicketsAPI.Services.Impl
{
    public class FlightTicketService : IFlightTicketService
    {
        private readonly IMongoCollection<EntityFlightTicket> _ticketsCollection;

        private static readonly Histogram MongoQueryDuration = Metrics.
            CreateHistogram("mongodb_query_duration_seconds", "Время выполнения запросов к MongoDB",
                new HistogramConfiguration
                {
                    LabelNames = ["operation", "collection"],
                    Buckets = [0.001, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1.0, 2.0]
                }
            );

        public FlightTicketService(IOptions<FlightTicketsDBSettings> flightTicketsDBSettings)
        {
            var mongoClient = new MongoClient(flightTicketsDBSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(flightTicketsDBSettings.Value.DatabaseName);
            _ticketsCollection = mongoDatabase.GetCollection<EntityFlightTicket>(flightTicketsDBSettings.Value.CollectionName);
        }

        public async Task<List<EntityFlightTicket>> GetAllAsync()
        {
            using(MongoQueryDuration.WithLabels("get_all", "flight_tickets").NewTimer())
            {
                return await _ticketsCollection.Aggregate().Sample(1000).ToListAsync();
            }
        }

        public async Task<EntityFlightTicket> GetAsync(string id)
        {
            using (MongoQueryDuration.WithLabels("get_by_id", "flight_tickets").NewTimer())
            {
                return await _ticketsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
        }
        
        public async Task CreateAsync(EntityFlightTicket entityFlightTicket)
        {
            using (MongoQueryDuration.WithLabels("insert", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.InsertOneAsync(entityFlightTicket);
            }
        }
            
        public async Task UpdateAsync(string id, EntityFlightTicket updatedEntityFlightTicket)
        {
            using (MongoQueryDuration.WithLabels("update", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.ReplaceOneAsync(x => x.Id == id, updatedEntityFlightTicket);
            }
        }
            
        public async Task DeleteAsync(string id)
        {
            using (MongoQueryDuration.WithLabels("delete", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.DeleteOneAsync(x => x.Id == id);
            }
        } 
            
        public async Task ClearAllAsync()
        {
            using (MongoQueryDuration.WithLabels("delete_all", "flight_tickets").NewTimer())
            {
                await _ticketsCollection.DeleteManyAsync(_ => true);
            }
        }
        public async Task<List<EntityFlightTicket>> GetFilteredAsync(FlightTicketFilter ftl)
        {
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
            using (MongoQueryDuration.WithLabels("get_filtered", "flight_tickets").NewTimer())
            {
                return await _ticketsCollection.Find(filter).ToListAsync();
            }            
        }
    }
}
