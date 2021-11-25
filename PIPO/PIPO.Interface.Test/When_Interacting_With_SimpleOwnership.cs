using System;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using PIPO.Contracts.SimpleOwnership;
using PIPO.Contracts.SimpleOwnership.ContractDefinition;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Interacting_With_SimpleOwnership
{
    private readonly string ContractAddress = "0x243e72b69141f6af525a9a5fd939668ee9f2b354";

    [Test]
    public async Task Should_Mint()
    {
        var simpleMintMessage = new SimpleMintFunction()
        {
            Receiver = "0x13f022d72158410433cbd66f5dd8bf6d2d129924"
        };
    
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var receipt = await new SimpleOwnershipService(connection.Web3, ContractAddress).SimpleMintRequestAndWaitForReceiptAsync(simpleMintMessage);
    
        receipt.Should().NotBeNull();
        receipt.Status.Value.Should().Be(1);
        Console.WriteLine($"Status -> {receipt.Status}");
    }
    
    // [Test]
    // public async Task Number_Of_Ownerships_Should_Be_Positive()
    // {
    //     var numberOfOwnershipsMessage = new SimpleOwnership.NumberOfOwnerships();
    //
    //     using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
    //     var handler = connection.Web3.Eth.GetContractQueryHandler<SimpleOwnership.NumberOfOwnerships>();
    //     var numberOfOwnerships = await handler.QueryAsync<BigInteger>(ContractAddress, numberOfOwnershipsMessage);
    //     
    //     Console.WriteLine($"Number of ownerships total -> {numberOfOwnerships}");
    // }
}