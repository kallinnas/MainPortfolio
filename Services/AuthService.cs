using MainPortfolio.Models;
using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Services.Interfaces;

namespace MainPortfolio.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthService(IUserRepository userRepository, JwtService jwtService, RefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
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

    private (string accessToken, string refreshToken)? GenerateAuthTokens(User user)
    {
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _refreshTokenService.Generate(user);
        return (accessToken, refreshToken);
    }

    public Task<bool> ValidateAccessTokenAsync(string token)
    {
        return Task.FromResult(_jwtService.ValidateAccessToken(token));
    }

    public Task<bool> ValidateRefreshTokenAsync(string token)
    {
        return Task.FromResult(_refreshTokenService.Validate(token));
    }

    public async Task<string?> GenerateNewAccessTokenAsync(string refreshToken)
    {
        var email = _refreshTokenService.GetUserEmailFromRefreshToken(refreshToken);
        var user = await _userRepository.GetUserByEmailAsync(email!);

        if (user != null)
        {
            return await Task.FromResult(_jwtService.GenerateAccessToken(user));
        }

        return null;
    }

}
