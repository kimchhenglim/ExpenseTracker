using ExpenseTracker.Application.Dtos.Incoming;
using ExpenseTracker.Application.Dtos.Outgoing;
using ExpenseTracker.Application.Entities;
using ExpenseTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ExpenseController(ExpenseService expenseService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateExpense(CreateExpenseRequest request)
    {
        ExpenseResponse createdExpense = await expenseService.CreateAsync(request);
        
        return CreatedAtAction(nameof(GetExpenses), new { id = createdExpense.Id }, createdExpense);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateExpense(Guid id, UpdateExpenseRequest request)
    {
        ExpenseResponse? updatedExpense = await expenseService.UpdateAsync(id, request);
        
        return updatedExpense == null ? NotFound() : CreatedAtAction(nameof(GetExpenses), new { id = updatedExpense.Id }, updatedExpense);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponse>> GetExpenses(Guid id)
    {
        ExpenseResponse? expense = await expenseService.GetByIdAsync(id);
        
        return expense == null ? NotFound() : Ok(expense);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetExpenses(string? range, DateTime? startDate, DateTime? endDate)
    {
        IReadOnlyList<ExpenseResponse> expenses = await expenseService.GetAllAsync(range, startDate, endDate);
        
        return Ok(expenses);
    }
}
