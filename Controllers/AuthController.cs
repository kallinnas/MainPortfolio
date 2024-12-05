using Microsoft.AspNetCore.Mvc;

using MainPortfolio.Security.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService)
    { _authService = authService; _refreshTokenService = refreshTokenService; }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthDto userDto)
    {
        var token = await _authService.LoginAsync(userDto);

        if (!string.IsNullOrEmpty(token))
        {
            _refreshTokenService.SetRefreshTokenCookie(token, Response.Cookies);
            return Ok(new { TokenStatus.Valid });
        }

        return Unauthorized(new { TokenStatus.NotFound });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrDto userDto)
    {
        var token = await _authService.RegisterAsync(userDto);

        if (!string.IsNullOrEmpty(token))
        {
            _refreshTokenService.SetRefreshTokenCookie(token, Response.Cookies);
            return Ok(new { TokenStatus.Valid });
        }

        return BadRequest(new { TokenStatus.NotFound });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _refreshTokenService.RemoveRefreshTokenCookie(Response.Cookies);
        return Ok(new { TokenStatus.Invalid });
    }


}

