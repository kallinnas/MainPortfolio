using Microsoft.AspNetCore.Mvc;

using MainPortfolio.Security.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenService _refreshTokenService;

    public TokenController(IRefreshTokenService refreshTokenService, IConfiguration configuration)
    { _refreshTokenService = refreshTokenService; _configuration = configuration; }

    [HttpGet("validateToken")]
    public IActionResult ValidateAccessToken()
    {
        var refreshToken = Request.Cookies[_configuration["Keys:RefreshToken"]!];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Ok(new { status = TokenStatus.NotFound });
        }

        if (!_refreshTokenService.ValidateToken(refreshToken))
        {
            return Ok(new { status = TokenStatus.Invalid });
        }

        return Ok(new { status = TokenStatus.Valid });
    }

}
