using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UserConfirmation.Data.Models;

namespace UserConfirmation.Data;

public class DbContext : IdentityDbContext<ApplicationUser>
{
    public DbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ConfirmationCode>()
            .HasKey(cc => new { cc.UserId, cc.Code }); // Composite key

        builder.Ignore<IdentityUserRole<string>>();
        builder.Ignore<IdentityUserClaim<string>>();
        builder.Ignore<IdentityUserToken<string>>();
        builder.Ignore<IdentityRoleClaim<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
    }
}
