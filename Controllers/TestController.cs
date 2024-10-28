using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainPortfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(new { message = "Hello from .NET 8 Web API" });
        }

        catch (Exception ex)
        {
            return Unauthorized(new { error = "RefreshTokenExpired", message = "Refresh token has expired." });
        }
    }
}
