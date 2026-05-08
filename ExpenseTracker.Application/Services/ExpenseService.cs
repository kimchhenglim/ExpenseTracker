using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Dtos.Incoming;
using ExpenseTracker.Application.Dtos.Outgoing;
using ExpenseTracker.Application.Entities;
using ExpenseTracker.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Services;

public class ExpenseService(AppDbContext appDbContext, ICurrentUserService appUser)
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request)
    {
        Expense expense = new Expense
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Amount = request.Amount,
            Category = request.Category,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = appUser.UserId
        };

        _appDbContext.Expenses.Add(expense);
        await _appDbContext.SaveChangesAsync();

        return Map(expense);
    }

    public async Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(string? range, DateTime? startDate, DateTime? endDate)
    {
        IQueryable<Expense> query = _appDbContext.Expenses.AsQueryable();
        query = query.Where(e => e.UserId == appUser.UserId);

        if (range != null)
        {
            if (range == "last week")
            {
                endDate = DateTime.UtcNow;
                startDate = endDate.Value.AddDays(-7);

                query = query.Where(e => e.Date >= startDate && e.Date <= endDate);
            }
            else if (range == "last month")
            {
                endDate = DateTime.UtcNow;
                startDate = endDate.Value.AddMonths(-1);
                query = query.Where(e => e.Date >= startDate && e.Date <= endDate);
            }
            else if (range == "last 3 months")
            {
                endDate = DateTime.UtcNow;
                startDate = endDate.Value.AddMonths(-3);
                query = query.Where(e => e.Date >= startDate && e.Date <= endDate);
            }
            else if (range == "custom")
            {
                if (startDate == null || endDate == null)
                    throw new ArgumentException("Start date and end date must be provided for custom range.");
                else 
                    query = query.Where(e => e.Date >= startDate && e.Date <= endDate);
            }
        }

        List<Expense> expenses = await query.OrderByDescending(e => e.Date).ToListAsync();

        return expenses.Select(Map).ToList();
    }

    public async Task<ExpenseResponse?> GetByIdAsync(Guid id)
    {
        Expense? expense = await _appDbContext.Expenses
            .Where(e => e.UserId == appUser.UserId && e.Id == id)
            .FirstOrDefaultAsync();
        
        return expense == null ? null : Map(expense);
    }

    public async Task<ExpenseResponse?> UpdateAsync(Guid id, UpdateExpenseRequest request)
    {
        Expense? expense = await _appDbContext.Expenses
            .Where(e => e.UserId == appUser.UserId && e.Id == id)
            .FirstOrDefaultAsync();

        if (expense == null)
        {
            return null;
        }

        expense.Title = request.Title;
        expense.Description = request.Description;
        expense.Amount = request.Amount;
        expense.Category = request.Category;
        expense.Date = request.Date;
        expense.UpdatedAt = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync();
        return Map(expense);
    }


    private static ExpenseResponse Map(Expense expense)
    {
        return new ExpenseResponse
        {
            Id = expense.Id,
            Title = expense.Title,
            Description = expense.Description,
            Amount = expense.Amount,
            Category = expense.Category,
            Date = expense.Date,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
