using MainPortfolio.Models;

namespace MainPortfolio.Services.Interfaces;

public interface IAuthService
{
    Task<(string accessToken, string refreshToken)?> LoginAsync(UserAuthDto userDto);
    Task<(string accessToken, string refreshToken)?> RegisterAsync(UserRegistrDto userDto);
    Task<string?> GenerateNewAccessTokenAsync(string refreshToken);
    Task<bool> ValidateAccessTokenAsync(string token);
    Task<bool> ValidateRefreshTokenAsync(string token);
}
