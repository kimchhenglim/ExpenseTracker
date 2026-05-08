using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Dtos.Incoming;
using ExpenseTracker.Application.Dtos.Outgoing;
using ExpenseTracker.Application.Entities;
using ExpenseTracker.Application.Options;
using ExpenseTracker.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Application.Services;

public class AuthService(
    AppDbContext appDbContext,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        User? user = await _appDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (user is null)
        {
            return null;
        }

        if (!_passwordHasher.Verify(request.Password, user.Password))
        {
            return null;
        }

        return await CreateAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> RefreshAsync(string refreshToken)
    {
        string tokenHash = _tokenService.HashRefreshToken(refreshToken);

        RefreshToken? storedToken = await _appDbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (storedToken is null || !storedToken.IsActive || storedToken.User is null)
        {
            return null;
        }

        storedToken.RevokedAt = DateTime.Now;

        AuthResponse response = await CreateAuthResponseAsync(storedToken.User);
        await _appDbContext.SaveChangesAsync();

        return response;
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        string tokenHash = _tokenService.HashRefreshToken(refreshToken);

        RefreshToken? storedToken = await _appDbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (storedToken is null || storedToken.RevokedAt is not null)
        {
            return false;
        }

        storedToken.RevokedAt = DateTime.Now;
        await _appDbContext.SaveChangesAsync();

        return true;
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(User user)
    {
        DateTime accessExpiresAt = DateTime.Now.AddMinutes(_jwtOptions.AccessTokenMinutes);
        DateTime refreshExpiresAt = DateTime.Now.AddDays(_jwtOptions.RefreshTokenDays);

        string accessToken = _tokenService.CreateAccessToken(user, accessExpiresAt);
        string refreshToken = _tokenService.CreateRefreshToken();
        string refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

        RefreshToken storedToken = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = DateTime.Now
        };

        _appDbContext.RefreshTokens.Add(storedToken);
        await _appDbContext.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = accessExpiresAt
        };
    }
}
