using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseTracker.Application.Services;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string CreateAccessToken(User user, DateTime expiresAtUtc)
    {
        string issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
        string audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");
        string key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name)
        ];

        SigningCredentials credentials = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string HashRefreshToken(string token)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(token);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
