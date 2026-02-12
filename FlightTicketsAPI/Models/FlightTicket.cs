using FlightTicketsAPI.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FlightTicketsAPI.Models
{
    [FlightDatesRange]
    public class FlightTicket
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Необходимо ввести номер рейса!")]
        [RegularExpression(@"^[A-Z]{2}\s\d{1,4}$", ErrorMessage = "Неверный формат номера рейса!'")]
        public string FlightNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Необходимо ввести код аэропорта отправления!")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Неверный формат кода аэропорта!'")]
        public string DepartureCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Необходимо ввести код аэропорта прибытия!")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Неверный формат кода аэропорта!'")]
        public string ArrivalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Необходимо ввести номер сиденья!")]
        [RegularExpression(@"^[A-Z]\d{2}$", ErrorMessage = "Неверный формат номера сиденья!'")]
        public string Seat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Необходимо ввести цену билета!")]
        [Range(0.01, 200000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Необходимо ввести дату и время отправления!")]
        public DateTimeOffset DepartureTime { get; set; }

        [Required(ErrorMessage = "Необходимо ввести дату и время прибытия!")]
        public DateTimeOffset ArrivalTime { get; set; }
    }
}
