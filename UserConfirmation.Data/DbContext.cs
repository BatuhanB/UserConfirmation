using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserConfirmation.Data.Models;

namespace UserConfirmation.Data;

public class DbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
{
    public DbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ConfirmationCode>()
            .HasKey(cc => new { cc.UserId, cc.Code }); // Composite key
        base.OnModelCreating(builder);
    }
}