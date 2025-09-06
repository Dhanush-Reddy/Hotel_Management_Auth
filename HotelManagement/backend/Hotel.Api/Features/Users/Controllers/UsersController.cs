using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hotel.Application.Features.Users.Interfaces;
using Hotel.Api.Features.Users.DTOs;

namespace Hotel.Api.Features.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;
        public UsersController(IUserService users) => _users = users;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? q = null)
        {
            var users = await _users.GetAllAsync(page, pageSize, q);
            var dtos = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                ActiveFlag = u.ActiveFlag,
                CreatedAt = u.CreatedAt
            });
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            var id = await _users.CreateUserAsync(req.Username, req.Role, req.Password);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user is null) return NotFound();

            var dto = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                ActiveFlag = user.ActiveFlag,
                CreatedAt = user.CreatedAt
            };
            return Ok(dto);
        }

        [HttpPut("{id:int}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromQuery] string role)
        {
            await _users.UpdateRoleAsync(id, role);
            return NoContent();
        }

        [HttpPut("{id:int}/active")]
        public async Task<IActionResult> SetActive(int id, [FromQuery] bool active)
        {
            await _users.SetActiveAsync(id, active);
            return NoContent();
        }

        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            await _users.ResetPasswordAsync(id, newPassword);
            return NoContent();
        }
    }
}
