using FlightTicketsAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace FlightTicketsAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FlightDatesRangeAttribute : ValidationAttribute
    {
        public FlightDatesRangeAttribute()
        {
            ErrorMessage = "Время прилета должно быть позже времени вылета!";
        }

        public override bool IsValid(object? value)
        {
            if(value is not FlightTicket ticket)
            {
                return false;
            }
            return ticket.ArrivalTime > ticket.DepartureTime;
        }
    }
}
