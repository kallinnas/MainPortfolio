using MainPortfolio.Models;

namespace MainPortfolio.Security.Services.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(UserAuthDto userDto);
    Task<string> RegisterAsync(UserRegistrDto userDto);
}
