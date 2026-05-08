using ExpenseTracker.Application.Dtos.Incoming;
using ExpenseTracker.Application.Dtos.Outgoing;
using ExpenseTracker.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        AuthResponse? response = await authService.LoginAsync(request);
        return response is null ? Unauthorized() : Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request)
    {
        AuthResponse? response = await authService.RefreshAsync(request.RefreshToken);
        return response is null ? Unauthorized() : Ok(response);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(RefreshTokenRequest request)
    {
        bool success = await authService.LogoutAsync(request.RefreshToken);
        return success ? NoContent() : NotFound();
    }
}
