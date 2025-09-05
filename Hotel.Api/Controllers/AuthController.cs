using System.Threading.Tasks;
using Hotel.Api.DTOs;
using Hotel.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _users;
        private readonly IJwtTokenFactory _jwt;

        public AuthController(IUserService users, IJwtTokenFactory jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
        {
            var user = await _users.AuthenticateAsync(req.Username, req.Password);
            if (user is null) return Unauthorized();

            var (token, exp) = _jwt.CreateToken(user.Id, user.Username, user.Role);
            return Ok(new LoginResponse { Token = token, Role = user.Role, ExpiresAtUtc = exp });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new {
                Id = User.FindFirst("sub")?.Value,          // subject claim from token
                Username = User.Identity?.Name,               // ClaimTypes.Name
                Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
            });
        }
    }
}
