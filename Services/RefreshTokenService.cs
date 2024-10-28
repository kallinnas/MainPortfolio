﻿using MainPortfolio.Models;
using System.Security.Cryptography;
using System.Text;

namespace MainPortfolio.Services;

public class RefreshTokenService
{
    private readonly IConfiguration _configuration;
    private readonly byte[] _refreshKey;

    public RefreshTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _refreshKey = Encoding.ASCII.GetBytes(_configuration["Jwt:RefreshKey"]!);
    }

    public string Generate(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        // Combining random num with user ID/email and a timestamp
        var tokenData = $"{Convert.ToBase64String(randomNumber)}|{user.Email}|{DateTime.UtcNow.Ticks}";
        var tokenBytes = Encoding.UTF8.GetBytes(tokenData);

        // Sign the refresh token data using a secret key (HMAC)
        using (var hmac = new HMACSHA256(_refreshKey))
        {
            var signature = hmac.ComputeHash(tokenBytes);
            return $"{tokenData}|{Convert.ToBase64String(signature)}";  // Return token + signature
        }
    }

    public bool Validate(string refreshToken)
    {
        var parts = refreshToken.Split('|');
        if (parts.Length != 4) return false;

        var tokenData = string.Join('|', parts.Take(3));  // The original data part
        var providedSignature = parts[3];  // The signature

        var tokenBytes = Encoding.UTF8.GetBytes(tokenData);

        // Recompute the HMAC signature and compare
        using (var hmac = new HMACSHA256(_refreshKey))
        {
            var computedSignature = hmac.ComputeHash(tokenBytes);
            return providedSignature == Convert.ToBase64String(computedSignature);
        }
    }

    public string? GetUserEmailFromRefreshToken(string refreshToken)
    {
        // Split the token into its parts (random part, email, timestamp, signature)
        var parts = refreshToken.Split('|');
        if (parts.Length != 4) return null;

        return parts[1];  // Extract the email part of the token
    }

}