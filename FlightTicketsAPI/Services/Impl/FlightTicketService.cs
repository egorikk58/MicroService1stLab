using MongoDB.Driver;
using FlightTicketsAPI.Models;
using Microsoft.Extensions.Options;

namespace FlightTicketsAPI.Services.Impl
{
    public class FlightTicketService : IFlightTicketService
    {
        private readonly IMongoCollection<EntityFlightTicket> _ticketsCollection;

        public FlightTicketService(IOptions<FlightTicketsDBSettings> flightTicketsDBSettings)
        {
            var mongoClient = new MongoClient(flightTicketsDBSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(flightTicketsDBSettings.Value.DatabaseName);
            _ticketsCollection = mongoDatabase.GetCollection<EntityFlightTicket>(flightTicketsDBSettings.Value.CollectionName);
        }

        public async Task<List<EntityFlightTicket>> GetAllAsync()
        {
            return await _ticketsCollection.Aggregate().Sample(1000).ToListAsync();
        }

        public async Task<EntityFlightTicket> GetAsync(string id) => 
            await _ticketsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(EntityFlightTicket entityFlightTicket) =>
            await _ticketsCollection.InsertOneAsync(entityFlightTicket);
        public async Task UpdateAsync(string id, EntityFlightTicket updatedEntityFlightTicket) =>
            await _ticketsCollection.ReplaceOneAsync(x => x.Id == id, updatedEntityFlightTicket);
        public async Task DeleteAsync(string id) => 
            await _ticketsCollection.DeleteOneAsync(x => x.Id == id);
        public async Task ClearAllAsync() =>
            await _ticketsCollection.DeleteManyAsync(_ => true);
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
            return await _ticketsCollection.Find(filter).ToListAsync();
        }
    }
}
