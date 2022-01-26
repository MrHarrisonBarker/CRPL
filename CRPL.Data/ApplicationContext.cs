using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Account;

public class ApplicationContext : DbContext
{
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<RegisteredWork> RegisteredWorks { get; set; }
    public DbSet<UserWork> UserWorks { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserWork>()
            .HasKey(t => new {t.UserId, t.WorkId});

        modelBuilder.Entity<UserWork>()
            .HasOne(pt => pt.UserAccount)
            .WithMany(p => p.UserWorks)
            .HasForeignKey(pt => pt.UserId);

        modelBuilder.Entity<UserWork>()
            .HasOne(pt => pt.RegisteredWork)
            .WithMany(t => t.UserWorks)
            .HasForeignKey(pt => pt.WorkId);
    }
}