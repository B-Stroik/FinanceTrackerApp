using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerApi.Data;

public class AuthFinanceTrackerDbContext : IdentityDbContext<IdentityUser>
{
    public AuthFinanceTrackerDbContext(DbContextOptions<AuthFinanceTrackerDbContext> options)
        : base(options)
    {
    }
}
