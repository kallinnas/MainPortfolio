using MainPortfolio.Models;

namespace MainPortfolio.Services.Interfaces;

public interface IAuthService
{
    Task<string?> LoginAsync(UserAuthDto userDto);
    Task<string?> RegisterAsync(UserRegistrDto userDto);
    Task<bool> ValidateTokenAsync(string token);
}
