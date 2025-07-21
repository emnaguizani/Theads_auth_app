[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    [HttpGet]
    [Authorize] // Requires authentication
    public IActionResult GetDashboard()
    {
        try 
        {
            // Simple test data
            var data = new
            {
                stats = new[]
                {
                    new { icon = "📊", title = "Test Stat", value = "123" }
                },
                recentActivity = new[]
                {
                    new { action = "Dashboard loaded", timestamp = DateTime.Now.ToString() }
                },
                notifications = new[]
                {
                    new { message = "Dashboard working!", time = "now" }
                }
            };

            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Alternative endpoint for testing without auth
    [HttpGet("test")]
    [AllowAnonymous]
    public IActionResult Test()
    {
        return Ok(new { message = "Dashboard controller is working!" });
    }
}