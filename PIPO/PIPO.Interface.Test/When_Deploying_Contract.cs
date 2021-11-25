using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using System.Numerics;
using Nethereum.Signer;
using PIPO.Contracts.SimpleOwnership;
using PIPO.Contracts.SimpleOwnership.ContractDefinition;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Deploying_Contract
{
    [Test]
    public async Task Should_Deploy_Copyright_Contract()
    {
        // using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        //
        //
        // var receipt = await Simple
        //
        // receipt.Should().NotBeNull();
        // receipt.Should().BeOfType<TransactionReceipt>();
        // Console.WriteLine($"Contract address -> {receipt.ContractAddress}");
    }

    [Test]
    public async Task Should_Deploy_SimpleOwnership_Contract()
    {
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));

        var receipt = await SimpleOwnershipService.DeployContractAndWaitForReceiptAsync(connection.Web3, new SimpleOwnershipDeployment() { Name = "Hello World" });
        
        receipt.Should().NotBeNull();
        receipt.Status.Should().Be(1);
        Console.WriteLine($"Contract address -> {receipt.ContractAddress}");
    }
}