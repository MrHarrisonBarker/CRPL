using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using PIPO.Contracts.SimpleOwnership;
using PIPO.Contracts.SimpleOwnership.ContractDefinition;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Deploying_Contract
{
    [Test]
    public async Task Should_Deploy_SimpleOwnership_Contract()
    {
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));

        var receipt = await SimpleOwnershipService.DeployContractAndWaitForReceiptAsync(connection.Web3, new SimpleOwnershipDeployment() { Name = "Hello World" });
        
        receipt.Should().NotBeNull();
        receipt.Status.Value.Should().Be(1);
        Console.WriteLine($"Contract address -> {receipt.ContractAddress}");
    }
}