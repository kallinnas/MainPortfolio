using Microsoft.AspNetCore.Mvc;
using MainPortfolio.Models;
using MainPortfolio.Services.Interfaces;

namespace MainPortfolio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) { _authService = authService; }

    [HttpPost("validateToken")]
    public async Task<IActionResult> ValidateToken([FromBody] TokenRequest request)
    {
        bool isValid = await _authService.ValidateAccessTokenAsync(request.Token);
        return Ok(isValid);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthDto userDto)
    {
        var tokens = await _authService.LoginAsync(userDto);

        if (tokens != null)
        {
            SetRefreshTokenCookie(tokens.Value.refreshToken);
            return Ok(new { tokens.Value.accessToken });
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrDto userDto)
    {
        var token = await _authService.RegisterAsync(userDto);

        if (token != null)
        {
            SetRefreshTokenCookie(token);
            return Ok(new { token });
        }

        return BadRequest("Email is already taken.");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful" });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken)) { return Unauthorized("No refresh token provided."); }

        if (!await _authService.ValidateRefreshTokenAsync(refreshToken)) { return Unauthorized("Invalid refresh token."); }

        return Ok(new { accessToken = _authService.GenerateNewAccessTokenAsync(refreshToken) });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(2)
            //Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}

