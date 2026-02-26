using FlightTicketsAPI.Models;
using FlightTicketsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FlightTicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightTicketsController : ControllerBase
    {
        private readonly FlightTicketService _entityFlightTicket;
        public FlightTicketsController(FlightTicketService flightTicketService)
        {
            _entityFlightTicket = flightTicketService;
        }
        [HttpGet]
        public async Task<ActionResult<List<DtoFlightTicket>>> GetAll()
        {
            var entities = await _entityFlightTicket.GetAllAsync();
            var dtos = entities.Select(e => new DtoFlightTicket
            {
                Id = e.Id,
                FlightNumber = e.FlightNumber,
                DepartureCode = e.DepartureCode,
                ArrivalCode = e.ArrivalCode,
                Seat = e.Seat,
                Price = e.Price,
                DepartureTime = e.DepartureTime,
                ArrivalTime = e.ArrivalTime
            }).ToList();

            return Ok(dtos);
        }

        //[HttpPost]
        //public ActionResult<FlightTicket> Create([FromBody] FlightTicket ft)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    long nextId = flightTickets.Any() ? flightTickets.Max(t => t.Id) + 1 : 1;
        //    ft.Id = nextId;
        //    flightTickets.Add(ft);
        //    return CreatedAtAction(nameof(GetById), new { id = ft.Id }, ft);
        //}

    //    [HttpPut("{id}")]
    //    public IActionResult Update(long id, [FromBody] FlightTicket newTicket)
    //    {
    //        if (id != newTicket.Id)
    //        {
    //            return BadRequest(new { ErrorMessage = "ID не совпадает с ID записи!" });
    //        }
    //        FlightTicket? oldTicket = flightTickets.FirstOrDefault(i => i.Id == id);
    //        if (oldTicket == null)
    //        {
    //            return NotFound(new { ErrorMessage = "Билет для обновления не найден" });
    //        }
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }
    //        oldTicket.FlightNumber = newTicket.FlightNumber;
    //        oldTicket.ArrivalTime = newTicket.ArrivalTime;
    //        oldTicket.ArrivalCode = newTicket.ArrivalCode;
    //        oldTicket.DepartureTime = newTicket.DepartureTime;
    //        oldTicket.DepartureCode = newTicket.DepartureCode;
    //        oldTicket.Price = newTicket.Price;
    //        oldTicket.Seat = newTicket.Seat;
    //        return Ok(oldTicket);
    //    }

    //    [HttpDelete("{id}")]
    //    public IActionResult Delete(long id)
    //    {
    //        FlightTicket? ft = flightTickets.FirstOrDefault(i => i.Id == id);
    //        if (ft == null)
    //        {
    //            return NotFound(new { ErrorMessage = "Билет с таким ID не найден!" });
    //        }
    //        flightTickets.Remove(ft);
    //        return NoContent();
    //    }
    }
}
