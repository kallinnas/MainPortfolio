using MainPortfolio.Models;
using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Services.Interfaces;

namespace ProductStoreSystemAPI.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<string?> LoginAsync(UserAuthDto userDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(userDto.Email);

        if (user != null && BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
        {
            return _jwtService.GenerateJwtToken(user);
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

        return _jwtService.GenerateJwtToken(newUser);
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        return Task.FromResult(_jwtService.ValidateToken(token));
    }
}
