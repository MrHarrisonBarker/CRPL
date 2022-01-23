using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Account;

public class ApplicationContext : DbContext
{
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<RegisteredWork> RegisteredWorks { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}