using Microsoft.AspNetCore.Mvc;
using MainPortfolio.Models;
using MainPortfolio.Services.Interfaces;

namespace MainPortfolio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("validateToken")]
    public async Task<IActionResult> ValidateToken([FromBody] TokenRequest request)
    {
        bool isValid = await _authService.ValidateTokenAsync(request.Token);
        return Ok(isValid);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthDto userDto)
    {
        var token = await _authService.LoginAsync(userDto);

        if (token != null)
        {
            return Ok(new { token });
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrDto userDto)
    {
        var token = await _authService.RegisterAsync(userDto);
        if (token != null)
        {
            return Ok(new { token });
        }

        return BadRequest("Email is already taken.");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful" });
    }

}

