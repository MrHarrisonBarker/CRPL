using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using PIPO.Contracts.SimpleOwnership;
using PIPO.Contracts.SimpleOwnership.ContractDefinition;
using ChangeOfOwnershipEventDTO = PIPO.Contracts.SimpleOwnership.ContractDefinition.ChangeOfOwnershipEventDTO;
using TransferOwnerFunction = PIPO.Contracts.SimpleOwnership.ContractDefinition.TransferOwnerFunction;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Interacting_With_SimpleOwnership
{
    private string ContractAddress;


    [SetUp]
    public async Task SetUp()
    {
        // Deploy contract
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var receipt = await SimpleOwnershipService.DeployContractAndWaitForReceiptAsync(connection.Web3, new SimpleOwnershipDeployment() { Name = "Hello World" });
        receipt.Status.Value.Should().Be(1);
        ContractAddress = receipt.ContractAddress;
    }

    [Test]
    public async Task Should_Estimate_Mint_Gas_Price()
    {
        var simpleMintMessage = new SimpleMintFunction()
        {
            Receiver = LawsOfNature.AccountId
        };

        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var service = new SimpleOwnershipService(connection.Web3, ContractAddress);
        var estimate = await service.ContractHandler.EstimateGasAsync(simpleMintMessage);

        estimate.Value.Should().BeGreaterThan(0);

        Console.WriteLine($"Estimate -> {estimate}");
    }

    [Test]
    public async Task Should_Mint()
    {
        var simpleMintMessage = new SimpleMintFunction()
        {
            Receiver = LawsOfNature.AccountId
        };

        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var receipt = await new SimpleOwnershipService(connection.Web3, ContractAddress).SimpleMintRequestAndWaitForReceiptAsync(simpleMintMessage);

        receipt.Should().NotBeNull();
        receipt.Status.Value.Should().Be(1);
        
        Console.WriteLine($"Status -> {receipt.Status}");
    }

    [Test]
    public async Task Should_Get_Number_Of_Ownerships()
    {
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var numberOfOwnerships = await new SimpleOwnershipService(connection.Web3, ContractAddress).NumberOfOwnershipsQueryAsync();
        numberOfOwnerships.Should().NotBeNull();
        
        Console.WriteLine($"There's {numberOfOwnerships}");
    }

    [Test]
    public async Task Number_Of_Ownerships_Should_Increase()
    {
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var numberOfOwnerships = await new SimpleOwnershipService(connection.Web3, ContractAddress).NumberOfOwnershipsQueryAsync();

        await new SimpleOwnershipService(connection.Web3, ContractAddress).SimpleMintRequestAndWaitForReceiptAsync(new SimpleMintFunction() { Receiver = LawsOfNature.AccountId });

        var newNumberOfOwnerships = await new SimpleOwnershipService(connection.Web3, ContractAddress).NumberOfOwnershipsQueryAsync();

        newNumberOfOwnerships.Should().BeGreaterThan(numberOfOwnerships);

        Console.WriteLine($"There used to be {numberOfOwnerships} now there's {newNumberOfOwnerships}");
    }

    [Test]
    public async Task Should_Get_Owner()
    {
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var owner = await new SimpleOwnershipService(connection.Web3, ContractAddress).OwnerQueryAsync();
        owner.Should().NotBeNull();
        
        Console.WriteLine($"The owner is {owner}");
    }

    [Test]
    public async Task Should_Transfer_Ownership()
    {
        var message = new TransferOwnerFunction()
        {
            To = LawsOfNature.OtherAccountId
        };
        
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var receipt = await new SimpleOwnershipService(connection.Web3, ContractAddress).TransferOwnerRequestAndWaitForReceiptAsync(message);

        receipt.Status.Value.Should().Be(1);
        
        var owner = await new SimpleOwnershipService(connection.Web3, ContractAddress).OwnerQueryAsync();
        owner.Should().BeEquivalentTo(LawsOfNature.OtherAccountId);
    }

    [Test]
    public async Task Should_Get_Transfer_Event()
    {
        var message = new TransferOwnerFunction()
        {
            To = LawsOfNature.OtherAccountId
        };
        
        using var connection = new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
        var receipt = await new SimpleOwnershipService(connection.Web3, ContractAddress).TransferOwnerRequestAndWaitForReceiptAsync(message);

        var decodedEvent = receipt.DecodeAllEvents<ChangeOfOwnershipEventDTO>().First();

        decodedEvent.Event.From.Should().BeEquivalentTo(LawsOfNature.AccountId);
        decodedEvent.Event.To.Should().BeEquivalentTo(LawsOfNature.OtherAccountId);
    }
}