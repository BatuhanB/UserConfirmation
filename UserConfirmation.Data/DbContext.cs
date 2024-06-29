using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserConfirmation.Data.Models;

namespace UserConfirmation.Data;

public class DbContext : IdentityDbContext<ApplicationUser>
{
    public DbContext(DbContextOptions options) : base(options)
    {
    }
}
