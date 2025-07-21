using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        
        public IActionResult GetDashboardData()
        {
            return Ok(new { message = "This is protected data" });
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult GetPublicData()
        {
            return Ok(new { message = "This is public data" });
        }
    }
}
