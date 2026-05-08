using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Application.Entities;

namespace ExpenseTracker.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor): ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    Guid ICurrentUserService.UserId
    {
        get
        {
            return Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        }
    }
}
