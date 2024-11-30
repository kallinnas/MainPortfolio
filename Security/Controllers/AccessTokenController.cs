using Microsoft.AspNetCore.Mvc;

using MainPortfolio.Security.Services.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Security.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessTokenController : ControllerBase
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AccessTokenController(IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService)
    {
        _accessTokenService = accessTokenService; _refreshTokenService = refreshTokenService;
    }

    [HttpPost("validateAccessToken")]
    public IActionResult ValidateToken([FromBody] TokenRequest request)
    {
        bool isValid = _accessTokenService.ValidateAccessToken(request.Token!);
        return Ok(isValid);
    }

    [HttpPost("updateAccessToken")]
    public IActionResult UpdateAccessToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { error = "RefreshTokenExpired", message = "Refresh token has expired." });
        }

        if (!_refreshTokenService.Validate(refreshToken))
        {
            return Unauthorized(new { error = "InvalidRefreshToken", message = "Invalid refresh token." });
        }

        return Ok(new { accessToken = _accessTokenService.GenerateNewAccessTokenAsync(refreshToken) });
    }
}
