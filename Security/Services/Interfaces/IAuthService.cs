using MainPortfolio.Models;

namespace MainPortfolio.Security.Services.Interfaces;

public interface IAuthService
{
    Task<(string accessToken, string refreshToken)?> LoginAsync(UserAuthDto userDto);
    Task<(string accessToken, string refreshToken)?> RegisterAsync(UserRegistrDto userDto);
    (string accessToken, string refreshToken)? GenerateAuthTokens(User user);
}
