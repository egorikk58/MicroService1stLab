namespace FlightTicketsAPI.Models
{
    public class FlightTicketFilter
    {
        public string? FlightNumber { get; set; }

        public string? DepartureCode { get; set; }

        public string? ArrivalCode { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public DateTime? DepartureDateFrom { get; set; }

        public DateTime? DepartureDateTo { get; set; }

    }
}
