//using FlightTicketsAPI.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Globalization;

//namespace FlightTicketsAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class FlightTicketsController : ControllerBase
//    {
//        private static List<FlightTicket> flightTickets = new List<FlightTicket>()
//        {
//            new FlightTicket
//            {
//                Id = 1,
//                FlightNumber = "SU 100",
//                DepartureCode = "SVO",
//                ArrivalCode = "LED",
//                Seat = "A01",
//                Price = 4500.00m,
//                DepartureTime = DateTimeOffset.Parse("20.05.2026 10:00 +03:00", CultureInfo.GetCultureInfo("ru-RU")),
//                ArrivalTime = DateTimeOffset.Parse("20.05.2026 11:30 +03:00", CultureInfo.GetCultureInfo("ru-RU"))
//            },
//            new FlightTicket
//            {
//                Id = 2,
//                FlightNumber = "SU 204",
//                DepartureCode = "SVO",
//                ArrivalCode = "PEK",
//                Seat = "B12",
//                Price = 62000.50m,
//                DepartureTime = DateTimeOffset.Parse("15.06.2026 18:45 +03:00", CultureInfo.GetCultureInfo("ru-RU")),
//                ArrivalTime = DateTimeOffset.Parse("16.06.2026 07:15 +08:00", CultureInfo.GetCultureInfo("ru-RU"))
//            },
//            new FlightTicket
//            {
//                Id = 3,
//                FlightNumber = "AF 1145",
//                DepartureCode = "CDG",
//                ArrivalCode = "SVO",
//                Seat = "C05",
//                Price = 38000.00m,
//                DepartureTime = DateTimeOffset.Parse("10.07.2026 09:20 +02:00", CultureInfo.GetCultureInfo("ru-RU")),
//                ArrivalTime = DateTimeOffset.Parse("10.07.2026 14:10 +03:00", CultureInfo.GetCultureInfo("ru-RU"))
//            },
//            new FlightTicket
//            {
//                Id = 4,
//                FlightNumber = "BA 0816",
//                DepartureCode = "LHR",
//                ArrivalCode = "JFK",
//                Seat = "F22",
//                Price = 85000.99m,
//                DepartureTime = DateTimeOffset.Parse("01.08.2026 13:00 +01:00", CultureInfo.GetCultureInfo("ru-RU")),
//                ArrivalTime = DateTimeOffset.Parse("01.08.2026 16:00 -04:00", CultureInfo.GetCultureInfo("ru-RU"))
//            }
//        };

//        [HttpGet]
//        public ActionResult<List<FlightTicket>> GetAll()
//        {
//            if(flightTickets.Count == 0)
//            {
//                return NoContent();
//            }
//            return Ok(flightTickets);
//        }

//        [HttpGet("{id}")]
//        public ActionResult<FlightTicket> GetById(long id)
//        {
//            FlightTicket? ft = flightTickets.FirstOrDefault(i => i.Id == id);
//            if(ft == null)
//            {
//                return NotFound(new { ErrorMessage = "Билета с таким ID не существует!" });
//            }
//            return Ok(ft);
//        }

//        [HttpPost]
//        public ActionResult<FlightTicket> Create([FromBody] FlightTicket ft)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//            long nextId = flightTickets.Any() ? flightTickets.Max(t => t.Id) + 1 : 1;
//            ft.Id = nextId;
//            flightTickets.Add(ft);
//            return CreatedAtAction(nameof(GetById), new { id = ft.Id }, ft);
//        }

//        [HttpPut("{id}")]
//        public IActionResult Update(long id, [FromBody] FlightTicket newTicket)
//        {
//            if(id != newTicket.Id)
//            {
//                return BadRequest(new { ErrorMessage = "ID не совпадает с ID записи!" });
//            }
//            FlightTicket? oldTicket = flightTickets.FirstOrDefault(i => i.Id == id);
//            if(oldTicket == null)
//            {
//                return NotFound(new { ErrorMessage = "Билет для обновления не найден" });
//            }
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//            oldTicket.FlightNumber = newTicket.FlightNumber;
//            oldTicket.ArrivalTime = newTicket.ArrivalTime;
//            oldTicket.ArrivalCode = newTicket.ArrivalCode;
//            oldTicket.DepartureTime = newTicket.DepartureTime;
//            oldTicket.DepartureCode = newTicket.DepartureCode;
//            oldTicket.Price = newTicket.Price;
//            oldTicket.Seat = newTicket.Seat;
//            return Ok(oldTicket);        
//        }

//        [HttpDelete("{id}")]
//        public IActionResult Delete(long id)
//        {
//            FlightTicket? ft = flightTickets.FirstOrDefault(i => i.Id == id);
//            if(ft == null)
//            {
//                return NotFound(new { ErrorMessage = "Билет с таким ID не найден!" });
//            }
//            flightTickets.Remove(ft);
//            return NoContent();
//        }
//    }
//}
