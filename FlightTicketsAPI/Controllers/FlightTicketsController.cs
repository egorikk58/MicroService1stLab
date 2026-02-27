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

        [HttpDelete("clearall")]
        public async Task<IActionResult> ClearAll()
        {
            await _ticketService.ClearAllAsync();
            return Ok(new { message = "Все элементы удалены из базы" });
        }

        [HttpPost("insertmany")]
        public async Task<IActionResult> InsertMany([FromBody] List<DtoFlightTicket> dtos)
        {
            if(dtos == null || dtos.Count == 0)
            {
                return BadRequest(new { message = "Список пуст" });
            }
            var entities = dtos.Select(i => new EntityFlightTicket
            {
                FlightNumber = i.FlightNumber,
                DepartureCode = i.DepartureCode,
                ArrivalCode = i.ArrivalCode,
                Seat = i.Seat,
                Price = i.Price,
                DepartureTime = i.DepartureTime,
                ArrivalTime = i.ArrivalTime
            }).ToList();
            await _ticketService.CreateManyAsync(entities);
            return Ok(new { message = "Элементы добавлены", count = entities.Count });
        }
    }
}