using MainPortfolio.Models;

namespace MainPortfolio.Security.Services.Interfaces;

public interface IRefreshTokenService
{
    string GenerateRefreshToken(User user);
    bool ValidateRefreshToken(string refreshToken);
    string? GetUserEmailFromRefreshToken(string refreshToken);
    void SetRefreshTokenCookie(string refreshToken, IResponseCookies cookies);
    void RemoveRefreshTokenCookie(IResponseCookies cookies);
}
