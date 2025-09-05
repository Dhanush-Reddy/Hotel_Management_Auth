using System.Threading.Tasks;
using Hotel.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly SqlConnectionFactory _factory;
        public HealthController(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        [HttpGet("db")]
        public async Task<IActionResult> CheckDb()
        {
            try
            {
                using var conn = _factory.CreateConnection();
                var result = await conn.ExecuteScalarAsync<int>("SELECT 1");
                return Ok(new { status = "Database connection OK", result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { status = "Database connection FAILED", error = ex.Message });
            }
        }
    }
}
