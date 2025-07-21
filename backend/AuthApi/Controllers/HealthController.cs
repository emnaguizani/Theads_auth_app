using Microsoft.AspNetCore.Mvc;
using AuthApi.Data;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HealthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CheckDatabaseConnection()
        {
            try
            {
                var canConnect = _context.Database.CanConnect();
                if (canConnect)
                    return Ok(new { status = "success", message = "Database connected successfully." });
                else
                    return StatusCode(500, new { status = "error", message = "Database connection failed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }
    }
}
