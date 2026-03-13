using FlightTicketsAPI.Models;

namespace FlightTicketsAPI.Mapper
{
    public class FlightTicketMapper
    {
        public static EntityFlightTicket MapDtoToEntity(DtoFlightTicket dto)
        {
            return new EntityFlightTicket
            {
                FlightNumber = dto.FlightNumber,
                DepartureCode = dto.DepartureCode,
                ArrivalCode = dto.ArrivalCode,
                Seat = dto.Seat,
                Price = dto.Price,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };
        }

        public static DtoFlightTicket MapEntityToDto(EntityFlightTicket e)
        {
            return new DtoFlightTicket
            {
                FlightNumber = e.FlightNumber,
                DepartureCode = e.DepartureCode,
                ArrivalCode = e.ArrivalCode,
                Seat = e.Seat,
                Price = e.Price,
                DepartureTime = e.DepartureTime,
                ArrivalTime = e.ArrivalTime
            };
        }
    }
}
