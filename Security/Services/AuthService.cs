using MainPortfolio.Security.Services.Interfaces;
using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Security.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(IUserRepository userRepository, IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository; _accessTokenService = accessTokenService; _refreshTokenService = refreshTokenService;
    }

    public async Task<(string accessToken, string refreshToken)?> LoginAsync(UserAuthDto userDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(userDto.Email);

        if (user != null && BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
        {
            return GenerateAuthTokens(user);
        }

        return null;
    }

    public async Task<(string accessToken, string refreshToken)?> RegisterAsync(UserRegistrDto userDto)
    {
        if (await _userRepository.IsEmailTakenAsync(userDto.Email)) { return null; }

        var newUser = new User(await _userRepository.IsAdmin(), userDto.Name, userDto.Email, BCrypt.Net.BCrypt.HashPassword(userDto.Password));

        await _userRepository.AddUserAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return GenerateAuthTokens(newUser);
    }

    public (string accessToken, string refreshToken)? GenerateAuthTokens(User user)
    {
        var accessToken = _accessTokenService.GenerateAccessToken(user);
        var refreshToken = _refreshTokenService.Generate(user);
        return (accessToken, refreshToken);
    }


}
