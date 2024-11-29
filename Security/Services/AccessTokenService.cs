using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

using MainPortfolio.Security.Services.Interfaces;
using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Security.Services;

public class AccessTokenService : IAccessTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;

    public AccessTokenService(IConfiguration configuration, IRefreshTokenService refreshTokenService, IUserRepository userRepository)
    {
        _configuration = configuration; _refreshTokenService = refreshTokenService; _userRepository = userRepository;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),

            //Expires = DateTime.UtcNow.AddHours(1),
            Expires = DateTime.UtcNow.AddSeconds(5),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<string?> GenerateNewAccessTokenAsync(string refreshToken)
    { // after expairation AccessToken updated by RefreshToken
        var email = _refreshTokenService.GetUserEmailFromRefreshToken(refreshToken);
        var user = await _userRepository.GetUserByEmailAsync(email!);

        if (user != null)
        {
            return await Task.FromResult(GenerateAccessToken(user));
        }

        return null;
    }

    public bool ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null) { return true; }

            return false;
        }

        catch (Exception)
        {
            return false;
        }
    }

}
