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
        private readonly FlightTicketService _ticketService;
        public FlightTicketsController(FlightTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DtoFlightTicket>>> GetAll()
        {
            var entities = await _ticketService.GetAllAsync();
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
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<DtoFlightTicket>> GetById(string id)
        {
            var entity = await _ticketService.GetAsync(id);
            if (entity is null)
            {
                return NotFound(new { message = $"Билет с ID {id} не найден" });
            }
            var dto = new DtoFlightTicket
            {
                Id = entity.Id,
                FlightNumber = entity.FlightNumber,
                DepartureCode = entity.DepartureCode,
                ArrivalCode = entity.ArrivalCode,
                Seat = entity.Seat,
                Price = entity.Price,
                DepartureTime = entity.DepartureTime,
                ArrivalTime = entity.ArrivalTime
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<DtoFlightTicket>> Create([FromBody] DtoFlightTicket dto)
        {
            var entity = new EntityFlightTicket
            {
                FlightNumber = dto.FlightNumber,
                DepartureCode = dto.DepartureCode,
                ArrivalCode = dto.ArrivalCode,
                Seat = dto.Seat,
                Price = dto.Price,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };

            await _ticketService.CreateAsync(entity);

            dto.Id = entity.Id;
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] DtoFlightTicket updatedDto)
        {
            var ticket = await _ticketService.GetAsync(id);

            if (ticket is null) return NotFound();

            var entityUpdate = new EntityFlightTicket
            {
                Id = id,
                FlightNumber = updatedDto.FlightNumber,
                DepartureCode = updatedDto.DepartureCode,
                ArrivalCode = updatedDto.ArrivalCode,
                Seat = updatedDto.Seat,
                Price = updatedDto.Price,
                DepartureTime = updatedDto.DepartureTime,
                ArrivalTime = updatedDto.ArrivalTime
            };

            await _ticketService.UpdateAsync(id, entityUpdate);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ticket = await _ticketService.GetAsync(id);

            if (ticket is null) return NotFound();

            await _ticketService.DeleteAsync(id);

            return NoContent();
        }

        [HttpDelete("test/clear")]
        public async Task<IActionResult> ClearAllTest()
        {
            await _ticketService.ClearAllAsync();
            return Ok(new { message = "Все элементы удалены из базы" });
        }

        [HttpPost("test/seed/{count}")]
        public async Task<IActionResult> SeedDataTest(int count)
        {
            var random = new Random();
            var airports = new[] { "SVO", "JFK", "LHR", "DXB", "HND" };
            var batch = new List<EntityFlightTicket>();

            for (int i = 0; i < count; i++)
            {
                batch.Add(new EntityFlightTicket
                {
                    FlightNumber = $"{(char)random.Next(65, 91)}{(char)random.Next(65, 91)} {random.Next(100, 9999)}",
                    DepartureCode = airports[random.Next(airports.Length)],
                    ArrivalCode = airports[random.Next(airports.Length)],
                    Seat = $"{(char)random.Next(65, 71)}{random.Next(1, 50)}",
                    Price = random.Next(1000, 100000),
                    DepartureTime = DateTime.UtcNow.AddDays(random.Next(1, 10)),
                    ArrivalTime = DateTime.UtcNow.AddDays(random.Next(11, 20))
                });
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            await _ticketService.CreateManyAsync(batch);
            sw.Stop();

            return Ok(new
            {
                message = $"Успешно добавлено {count} элементов",
                timeElapsedSeconds = sw.Elapsed.TotalSeconds
            });
        }
    }
}