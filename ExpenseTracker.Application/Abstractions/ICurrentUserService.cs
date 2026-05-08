using ExpenseTracker.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Abstractions;

public interface ICurrentUserService
{
    public Guid UserId { get; }
}
