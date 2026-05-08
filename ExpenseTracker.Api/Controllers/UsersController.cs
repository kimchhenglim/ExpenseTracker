using Azure;
using ExpenseTracker.Application.Dtos.Incoming;
using ExpenseTracker.Application.Dtos.Outgoing;
using ExpenseTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace ExpenseTracker.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UsersController(UserService userService, AuthService authService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        List<UserResponse> users = await userService.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        UserResponse? user = await userService.GetByIdAsync(id);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> CreateUser(CreateUserRequest request)
    {
        UserResponse createdUser = await userService.CreateAsync(request);
        return CreatedAtAction("GetUsers", new { id = createdUser.Id}, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(Guid id, UpdateUserRequest request)
    {
        UserResponse? updated = await userService.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }
}
