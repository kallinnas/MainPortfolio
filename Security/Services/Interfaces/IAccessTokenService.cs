using MainPortfolio.Models;

namespace MainPortfolio.Security.Services.Interfaces;

public interface IAccessTokenService
{
    string GenerateAccessToken(User user);
    bool ValidateAccessToken(string token);
    Task<string?> GenerateNewAccessTokenAsync(string refreshToken);
}
