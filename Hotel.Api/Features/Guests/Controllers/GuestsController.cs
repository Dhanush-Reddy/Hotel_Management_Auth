using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel.Application.Features.Guests.Interfaces;
using Hotel.Api.Features.Guests.DTOs;

namespace Hotel.Api.Features.Guests.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guests;
        public GuestsController(IGuestService guests) => _guests = guests;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GuestCreateRequest req)
        {
            var id = await _guests.CreateAsync(req.FullName, req.Phone, req.Email, req.IdProof);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var g = await _guests.GetAsync(id);
            if (g is null) return NotFound();
            return Ok(new GuestResponse {
                Id = g.Id, FullName = g.FullName, Phone = g.Phone, Email = g.Email, IdProof = g.IdProof, CreatedAt = g.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? q = null)
        {
            var list = await _guests.SearchAsync(page, pageSize, q);
            var dtos = list.Select(g => new GuestResponse {
                Id = g.Id, FullName = g.FullName, Phone = g.Phone, Email = g.Email, IdProof = g.IdProof, CreatedAt = g.CreatedAt
            });
            return Ok(dtos);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GuestUpdateRequest req)
        {
            await _guests.UpdateAsync(id, req.FullName, req.Phone, req.Email, req.IdProof);
            return NoContent();
        }
    }
}

