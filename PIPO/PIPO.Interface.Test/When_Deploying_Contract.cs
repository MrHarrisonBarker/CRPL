using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts;
using NUnit.Framework;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Deploying_Contract
{
    [Test]
    public async Task Should_Deploy_Copyright_Contract()
    {
        using var deployer = new ContractDeployer<CopyrightsContractDeployment>(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));

        var receipt = await deployer.DeployAsync(new CopyrightsContractDeployment());

        receipt.Should().NotBeNull();
        receipt.Should().BeOfType<TransactionReceipt>();
    }
}