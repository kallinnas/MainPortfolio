using Microsoft.AspNetCore.Mvc;

using MainPortfolio.Security.Services.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Security.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService)
    {
        _authService = authService; _refreshTokenService = refreshTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthDto userDto)
    {
        var tokens = await _authService.LoginAsync(userDto);

        if (tokens != null)
        {
            _refreshTokenService.SetRefreshTokenCookie(tokens.Value.refreshToken, Response.Cookies);
            return Ok(new { tokens.Value.accessToken });
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrDto userDto)
    {
        var tokens = await _authService.RegisterAsync(userDto);

        if (tokens != null)
        {
            _refreshTokenService.SetRefreshTokenCookie(tokens.Value.refreshToken, Response.Cookies);
            return Ok(new { tokens.Value.accessToken });
        }

        return BadRequest("Email is already taken.");
    }

    [HttpPost("logout")]
    public IActionResult Logout() { return Ok(new { message = "Logout successful" }); }

}

