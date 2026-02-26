using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightTicketsAPI.Models
{
    public class EntityFlightTicket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureCode { get; set; } = string.Empty;
        public string ArrivalCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.Decimal128)] 
        public decimal Price { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

    }
}
