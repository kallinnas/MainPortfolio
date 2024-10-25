using MainPortfolio.Models;

namespace MainPortfolio.Services.Interfaces;

public interface IAuthService
{
    Task<string?> GenerateNewAccessTokenAsync(string refreshToken);
    Task<(string accessToken, string refreshToken)?> LoginAsync(UserAuthDto userDto);
    Task<string?> RegisterAsync(UserRegistrDto userDto);
    Task<bool> ValidateAccessTokenAsync(string token);
    Task<bool> ValidateRefreshTokenAsync(string token);
}
