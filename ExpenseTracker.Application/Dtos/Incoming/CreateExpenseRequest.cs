using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Dtos.Incoming;

public class CreateExpenseRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public DateTime Date { get; set; }
}
