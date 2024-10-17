using Microsoft.AspNetCore.Mvc;

namespace MainPortfolio.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello from .NET 8 Web API" });
    }
}
