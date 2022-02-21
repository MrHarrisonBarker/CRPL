using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Account;

public class ApplicationContext : DbContext
{
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<RegisteredWork> RegisteredWorks { get; set; }
    public DbSet<UserWork> UserWorks { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<CopyrightRegistrationApplication> CopyrightRegistrationApplications { get; set; }
    public DbSet<OwnershipRestructureApplication> OwnershipRestructureApplications { get; set; }
    public DbSet<DisputeApplication> DisputeApplications { get; set; }

    public DbSet<UserApplication> UserApplications { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //// UserAccount <-> Work
        
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
        
        //// UserAccount <-> Application
        
        modelBuilder.Entity<UserApplication>()
            .HasKey(t => new {t.UserId, t.ApplicationId});

        modelBuilder.Entity<UserApplication>()
            .HasOne(pt => pt.UserAccount)
            .WithMany(p => p.Applications)
            .HasForeignKey(pt => pt.UserId);

        modelBuilder.Entity<UserApplication>()
            .HasOne(pt => pt.Application)
            .WithMany(t => t.AssociatedUsers)
            .HasForeignKey(pt => pt.ApplicationId);
    }
}