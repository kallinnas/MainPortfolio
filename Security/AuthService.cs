using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Security.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Security;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(IUserRepository userRepository, IRefreshTokenService refreshTokenService)
    { _userRepository = userRepository; _refreshTokenService = refreshTokenService; }

    public async Task<string> LoginAsync(UserAuthDto userDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(userDto.Email);

        if (user != null && BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
        {
            return _refreshTokenService.GenerateToken(user);
        }

        return "";
    }

    public async Task<string> RegisterAsync(UserRegistrDto userDto)
    {
        if (await _userRepository.IsEmailTakenAsync(userDto.Email)) { return ""; }

        var newUser = new User(await _userRepository.IsAdmin(), userDto.Name, userDto.Email, BCrypt.Net.BCrypt.HashPassword(userDto.Password));

        await _userRepository.AddUserAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return _refreshTokenService.GenerateToken(newUser);
    }


}
