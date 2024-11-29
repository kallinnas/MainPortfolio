using MainPortfolio.Security.Models;
using MainPortfolio.Security.Services;
using MainPortfolio.Security.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MainPortfolio.Security.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RefreshTokenController : ControllerBase
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenController(IRefreshTokenService refreshTokenService) { _refreshTokenService = refreshTokenService; }

    [HttpGet("validate")]
    public IActionResult ValidateRefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Ok(new { status = RefreshTokenState.NotFound, message = "Sign in or Login to start a new session." });
        }

        var isValid = _refreshTokenService.Validate(refreshToken);

        if (!isValid)
        {
            return Ok(new { status = RefreshTokenState.Invalid, message = "Relogin please, your session has been expired." });
        }

        return Ok(new { status = RefreshTokenState.Valid, message = "Refresh token is valid." });
    }


}
