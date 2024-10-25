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
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _refreshTokenService.Generate(user);
            return (accessToken, refreshToken);
        }

        return null;
    }

    public async Task<string?> RegisterAsync(UserRegistrDto userDto)
    {
        if (await _userRepository.IsEmailTakenAsync(userDto.Email))
        {
            return null;
        }

        var newUser = new User
        {
            Email = userDto.Email,
            Name = userDto.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Role = await _userRepository.IsAdmin()
        };

        await _userRepository.AddUserAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return _jwtService.GenerateAccessToken(newUser);
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
