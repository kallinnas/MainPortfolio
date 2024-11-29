using MainPortfolio.Models;

namespace MainPortfolio.Security.Services.Interfaces;

public interface IRefreshTokenService
{
    string Generate(User user);
    bool Validate(string refreshToken);
    string? GetUserEmailFromRefreshToken(string refreshToken);
    void SetRefreshTokenCookie(string refreshToken, IResponseCookies cookies);
}
