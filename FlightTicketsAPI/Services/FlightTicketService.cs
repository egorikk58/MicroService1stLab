using MongoDB.Driver;
using FlightTicketsAPI.Models;
using Microsoft.Extensions.Options;

namespace FlightTicketsAPI.Services
{
    public class FlightTicketService
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
    }
}
