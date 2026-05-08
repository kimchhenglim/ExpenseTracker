using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Dtos.Incoming;
using ExpenseTracker.Application.Dtos.Outgoing;
using ExpenseTracker.Application.Entities;
using ExpenseTracker.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Services;

public class UserService(AppDbContext appDbContext, IPasswordHasher passwordHasher)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        DateTime now = DateTime.UtcNow;

        User user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = _passwordHasher.Hash(request.Password),
            CreatedAt = now,
            UpdatedAt = now
        };

        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        return Map(user);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        User? user = await _appDbContext.Users.FindAsync(id);
        
        if (user is null) return null;
        
        user.Name = request.Name;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;
        
        _appDbContext.Users.Update(user);
        await _appDbContext.SaveChangesAsync();
        
        return Map(user);
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        List<User> users = await _appDbContext.Users.OrderByDescending(o => o.CreatedAt).ToListAsync();
        return users.Select(Map).ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        User? user = await _appDbContext.Users.FindAsync(id);
        return user is null ? null : Map(user);
    }

    private static UserResponse Map(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
