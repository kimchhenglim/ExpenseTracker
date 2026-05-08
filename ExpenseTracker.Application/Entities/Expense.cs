using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Entities;

public class Expense : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public DateTime Date { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
