using MainPortfolio.Models;

namespace MainPortfolio.Security.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}

public interface IAccessTokenService : ITokenService
{
    Task<string?> GenerateNewAccessTokenAsync(string refreshToken);
}

public interface IRefreshTokenService : ITokenService
{
    void SetRefreshTokenCookie(string refreshToken, IResponseCookies cookies);
    void RemoveRefreshTokenCookie(IResponseCookies cookies);
}