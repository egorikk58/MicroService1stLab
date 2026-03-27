using FlightTicketsAPI.Mapper;
using FlightTicketsAPI.Models;
using FlightTicketsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FlightTicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightTicketsController : ControllerBase
    {
        private readonly IFlightTicketService _ticketService;
        public FlightTicketsController(IFlightTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DtoFlightTicket>>> GetAll()
        {
            var entities = await _ticketService.GetAllAsync();
            var dtos = entities.Select(FlightTicketMapper.MapEntityToDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<DtoFlightTicket>> GetById(string id)
        {
            if(!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Некорректный формат ID" });
            }
            var entity = await _ticketService.GetAsync(id);
            if (entity is null)
            {
                return NotFound(new { message = $"Билет с ID {id} не найден" });
            }
            var dto = FlightTicketMapper.MapEntityToDto(entity);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<DtoFlightTicket>> Create([FromBody] DtoFlightTicket dto)
        {
            var entity = FlightTicketMapper.MapDtoToEntity(dto);
            await _ticketService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] DtoFlightTicket updatedDto)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Некорректный формат ID" });
            }
            var ticket = await _ticketService.GetAsync(id);
            if (ticket is null) return NotFound();
            var entityUpdate = FlightTicketMapper.MapDtoToEntity(updatedDto);
            entityUpdate.Id = id;
            await _ticketService.UpdateAsync(id, entityUpdate);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "Некорректный формат ID" });
            }
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

        [HttpGet("filter")]
        public async Task<ActionResult<List<DtoFlightTicket>>> GetFiltered([FromQuery] FlightTicketFilter ftl)
        {
            var entities = await _ticketService.GetFilteredAsync(ftl);
            var dtos = entities.Select(FlightTicketMapper.MapEntityToDto).ToList();
            return Ok(dtos);
        }
    }
}