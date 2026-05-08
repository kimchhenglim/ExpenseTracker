using ExpenseTracker.Application.Entities;

namespace ExpenseTracker.Application.Abstractions;

public interface ITokenService
{
    string CreateAccessToken(User user, DateTime expiresAtUtc);
    string CreateRefreshToken();
    string HashRefreshToken(string token);
}
