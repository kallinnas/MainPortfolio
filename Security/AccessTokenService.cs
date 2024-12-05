using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using MainPortfolio.Repositories.Interfaces;
using MainPortfolio.Security.Interfaces;
using MainPortfolio.Models;

namespace MainPortfolio.Security;

public class AccessTokenService : IAccessTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AccessTokenService(IConfiguration configuration, IUserRepository userRepository)
    { _configuration = configuration; _userRepository = userRepository; }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]),
            Expires = DateTime.UtcNow.AddSeconds(30),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
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
            return userIdClaim != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string?> GenerateNewAccessTokenAsync(string refreshToken)
    { // after expairation AccessToken updated by RefreshToken
        var email = GetUserEmailFromToken(refreshToken);
        var user = await _userRepository.GetUserByEmailAsync(email!);

        if (user != null)
        {
            return await Task.FromResult(GenerateToken(user));
        }

        return null;
    }

    public string? GetUserEmailFromToken(string refreshToken)
    {   // Split the token into its parts (random part, email, timestamp, signature)
        var parts = refreshToken.Split('|');
        if (parts.Length != 4) return null;
        return parts[1];  // Extract the email part
    }
}
