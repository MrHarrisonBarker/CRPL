using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.ContractDeployment;

// A database context used by EfCore to create and interact with tables
public class ContractContext : DbContext
{
    // List of deployed smart contracts
    public DbSet<DeployedContract> DeployedContracts { get; set; }
    
    public ContractContext(DbContextOptions<ContractContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}