namespace CRPL.Data.ContractDeployment;

public interface IContractRepository
{
    public Task Init();
    public DeployedContract DeployedContract(CopyrightContract contract);
    public Task DeployContract(DeployedContract contract);
}

public class ContractRepository : IContractRepository
{
    private readonly ContractContext ContractContext;

    private Dictionary<CopyrightContract, DeployedContract> DeployedContracts;

    public ContractRepository(ContractContext contractContext)
    {
        ContractContext = contractContext;
        
        getContracts();
    }

    private void getContracts()
    {
        DeployedContracts = ContractContext.DeployedContracts.ToDictionary(x => x.Type, x => x);
    }

    public Task Init()
    {
        // runs with a clear workspace to deploy all contracts needed
        throw new NotImplementedException();
    }

    public DeployedContract DeployedContract(CopyrightContract contract)
    {
        if (!DeployedContracts.ContainsKey(contract)) throw new Exception("That contract doesn't exist");
        
        return DeployedContracts[contract];
    }

    public async Task DeployContract(DeployedContract contract)
    {
        // replaces old contract

        ContractContext.DeployedContracts.Remove(ContractContext.DeployedContracts.First(x => x.Type == contract.Type));

        await ContractContext.DeployedContracts.AddAsync(contract);
    }
}