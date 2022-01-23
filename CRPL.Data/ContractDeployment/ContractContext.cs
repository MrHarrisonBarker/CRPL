using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.ContractDeployment;

public class ContractContext : DbContext
{
    public DbSet<DeployedContract> DeployedContracts { get; set; }
    
    public ContractContext(DbContextOptions<ContractContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}