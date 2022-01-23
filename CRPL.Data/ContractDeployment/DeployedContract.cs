namespace CRPL.Data.ContractDeployment;

public class DeployedContract
{
    public CopyrightContract Type { get; set; }
    public Guid Id { get; set; }
    public string Address { get; set; }
    public DateTime Deployed { get; set; }
}