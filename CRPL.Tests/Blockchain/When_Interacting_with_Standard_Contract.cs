using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Contracts.Standard;
using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data;
using CRPL.Data.BlockchainUtils;
using FluentAssertions;
using Nethereum.Model;
using NUnit.Framework;

namespace CRPL.Tests.Blockchain;

[TestFixture]
public class When_Interacting_with_Standard_Contract
{
    private string ContractAddress;
    
    [SetUp]
    [Ignore("need ci blockchain")]
    public async Task SetUp()
    {
        // Deploy contract
        using var connection = TestConstants.PrivateTestConnection();
        var receipt = await StandardService.DeployContractAndWaitForReceiptAsync(connection.Web3, new StandardDeployment());
        receipt.Status.Value.Should().Be(1);
        ContractAddress = receipt.ContractAddress;
    }
    
    [Test]
    [Ignore("need ci blockchain")]
    public async Task Should_Register_New_Copyright()
    {
        using var connection = TestConstants.PrivateTestConnection();

        var receipt = await new StandardService(connection.Web3, ContractAddress).RegisterRequestAndWaitForReceiptAsync(new Register1Function()
        {
            To = new List<OwnershipStructure>()
            {
              new()
              {
                  Owner = TestConstants.TestAccountAddress,
                  Share = 100
              }  
            },
            Def = new Meta()
            {
                Title = "Hello world",
                Expires = 0,
                Registered = 0,
                LegalMeta = "legal",
                WorkHash = "hash",
                WorkUri = "uri"
            }
        });
        
        receipt.Should().NotBeNull();
        receipt.Status.Value.Should().Be(1);
        
        Console.WriteLine($"Status -> {receipt.Status}");
    }
}