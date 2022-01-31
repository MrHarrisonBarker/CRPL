using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Contracts.Standard;
using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data;
using CRPL.Data.BlockchainUtils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Model;
using NUnit.Framework;

namespace CRPL.Tests.Blockchain;

[TestFixture]
public class When_Interacting_with_Standard_Contract
{
    private string ContractAddress;
    private BlockchainConnection BlockchainConnection;
    
    [SetUp]
    [Ignore("need ci blockchain")]
    public async Task SetUp()
    {
        var appSettings = Options.Create(new AppSettings()
        {
            Chains = new List<Chain>()
            {
                new()
                {
                    Name = "LOCAL",
                    Url = "http://localhost:8545",
                    Id = "444444444500"
                }
            },
            SystemAccount = new SystemAccount()
            {
                AccountId = TestConstants.TestAccountAddress,
                PrivateKey = TestConstants.TestAccountPrivateKey
            }
        });
        
        Environment.SetEnvironmentVariable("CURRENT_CHAIN","LOCAL");
        
        BlockchainConnection = new BlockchainConnection(new Logger<BlockchainConnection>(new LoggerFactory()), appSettings);
        
        // Deploy contract
        var receipt = await StandardService.DeployContractAndWaitForReceiptAsync(BlockchainConnection.Web3(), new StandardDeployment());
        receipt.Status.Value.Should().Be(1);
        ContractAddress = receipt.ContractAddress;
    }
    
    [Test]
    [Ignore("need ci blockchain")]
    public async Task Should_Register_New_Copyright()
    {
        var receipt = await new StandardService(BlockchainConnection.Web3(), ContractAddress).RegisterRequestAndWaitForReceiptAsync(new Register1Function()
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