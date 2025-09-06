using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel.Application.Features.Rooms.Interfaces;
using Hotel.Api.Features.Rooms.DTOs;

namespace Hotel.Api.Features.Rooms.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authenticated users can view; writes restricted below
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _rooms;

        public RoomsController(IRoomService rooms)
        {
            _rooms = rooms;
        }

        // GET /api/rooms?page=1&pageSize=50&q=10&status=Available
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? q = null, [FromQuery] string? status = null)
        {
            var items = await _rooms.ListAsync(page, pageSize, q, status);
            var dtos = items.Select(r => new RoomResponse
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Capacity = r.Capacity,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var r = await _rooms.GetAsync(id);
            if (r is null) return NotFound();
            return Ok(new RoomResponse {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Capacity = r.Capacity,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RoomCreateRequest req)
        {
            var id = await _rooms.CreateAsync(req.RoomNumber, req.Capacity);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomUpdateRequest req)
        {
            await _rooms.UpdateAsync(id, req.RoomNumber, req.Capacity);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rooms.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] RoomStatusRequest req)
        {
            await _rooms.SetStatusAsync(id, req.Status);
            return NoContent();
        }
    }
}

