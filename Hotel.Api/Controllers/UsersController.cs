using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hotel.Application.Interfaces;
using Hotel.Api.DTOs;

namespace Hotel.Api.Controllers
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
            => Ok(await _users.GetAllAsync(page, pageSize, q));

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
            return user is null ? NotFound() : Ok(user);
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

