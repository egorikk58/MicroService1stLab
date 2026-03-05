using FlightTicketsAPI.Models;

namespace FlightTicketsAPI.Services
{
    public interface IFlightTicketService
    {
        Task<List<EntityFlightTicket>> GetAllAsync();
        Task<EntityFlightTicket> GetAsync(string id);
        Task CreateAsync(EntityFlightTicket entityFlightTicket);
        Task UpdateAsync(string id, EntityFlightTicket updatedEntityFlightTicket);
        Task DeleteAsync(string id);
        Task ClearAllAsync();
        Task<List<EntityFlightTicket>> GetFilteredAsync(FlightTicketFilter ftl);
    }
}
