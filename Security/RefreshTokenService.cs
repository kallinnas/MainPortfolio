using System.Security.Cryptography;
using System.Text;

using MainPortfolio.Security.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Security;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IConfiguration _configuration;
    private readonly byte[] _refreshKey;

    public RefreshTokenService(IConfiguration configuration)
    { _configuration = configuration; _refreshKey = Encoding.ASCII.GetBytes(_configuration["Jwt:RefreshKey"]!); }

    public string GenerateToken(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        // Combining random num with user ID/email and a timestamp
        var tokenData = $"{Convert.ToBase64String(randomNumber)}|{user.Email}|{DateTime.UtcNow.Ticks}";
        var tokenBytes = Encoding.UTF8.GetBytes(tokenData);

        // Sign refresh token data using a secret key (HMAC)
        using (var hmac = new HMACSHA256(_refreshKey))
        {
            var signature = hmac.ComputeHash(tokenBytes);
            return $"{tokenData}|{Convert.ToBase64String(signature)}";
        }
    }

    public bool ValidateToken(string refreshToken)
    {
        var parts = refreshToken.Split('|');
        if (parts.Length != 4) return false;

        var tokenData = string.Join('|', parts.Take(3));
        var providedSignature = parts[3];

        var tokenBytes = Encoding.UTF8.GetBytes(tokenData);

        // Recompute the HMAC signature and compare
        using (var hmac = new HMACSHA256(_refreshKey))
        {
            var computedSignature = hmac.ComputeHash(tokenBytes);
            return providedSignature == Convert.ToBase64String(computedSignature);
        }
    }

    public void SetRefreshTokenCookie(string refreshToken, IResponseCookies cookies)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(3)
        };

        cookies.Append(_configuration["Keys:RefreshToken"]!, refreshToken, cookieOptions);
    }

    public void RemoveRefreshTokenCookie(IResponseCookies cookies)
    {
        cookies.Append(_configuration["Keys:RefreshToken"]!, "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });
    }
}
