using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data;
using CRPL.Data.BlockchainUtils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CRPL.Tests.Blockchain;

[TestFixture]
[Ignore("need ci blockchain")]
public class When_Interacting_with_Standard_Contract
{
    private string ContractAddress;
    private BlockchainConnection BlockchainConnection;
    
    [SetUp]
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
                    Id = "444444444500",
                    SystemAccount = new SystemAccount()
                    {
                        AccountId = TestConstants.TestAccountAddress,
                        PrivateKey = TestConstants.TestAccountPrivateKey
                    }
                }
            }
        });
        
        Environment.SetEnvironmentVariable("CURRENT_CHAIN","LOCAL");
        
        BlockchainConnection = new BlockchainConnection(new Logger<BlockchainConnection>(new LoggerFactory()), appSettings);
        
        // Deploy contract
        var receipt = await Contracts.Copyright.CopyrightService.DeployContractAndWaitForReceiptAsync(BlockchainConnection.Web3(), new CopyrightDeployment());
        receipt.Status.Value.Should().Be(1);
        ContractAddress = receipt.ContractAddress;
    }
    
    [Test]
    public async Task Should_Register_New_Copyright()
    {
        var funcMessage = new RegisterFunction()
        {
            To = new List<OwnershipStakeContract>
            {
                new()
                {
                    Owner = TestConstants.TestAccountAddress,
                    Share = 100
                }
            },
            Meta = new Meta()
            {
                Title = "Hello world",
                Expires = 0,
                Registered = 0,
                LegalMeta = "legal",
                WorkHash = "hash",
                WorkUri = "uri",
                WorkType = "Image",
                Protections = new Protections()
                {
                    Authorship = true,
                    CommercialAdaptation = true,
                    CommercialDistribution = true,
                    CommercialPerformance = true,
                    CommercialReproduction = true,
                    NonCommercialAdaptation = true,
                    NonCommercialDistribution = true,
                    NonCommercialPerformance = true,
                    NonCommercialReproduction = true,
                    ReviewOrCrit = true
                }
            }
        };

        var receipt = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractAddress).RegisterRequestAndWaitForReceiptAsync(funcMessage);

        receipt.Should().NotBeNull();
        receipt.Status.Value.Should().Be(1);

        var res = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractAddress).CopyrightMetaQueryAsync(1);

        res.ReturnValue1.Title.Should().BeEquivalentTo("Hello world");
        
        Console.WriteLine($"Status -> {receipt.Status}");
    }

    [Test]
    public async Task Should_Get_Ownership()
    {
        var funcMessage = new RegisterFunction()
        {
            To = new List<OwnershipStakeContract>
            {
                new()
                {
                    Owner = TestConstants.TestAccountAddress,
                    Share = 100
                }
            },
            Meta = new Meta()
            {
                Title = "Hello world",
                Expires = 0,
                Registered = 0,
                LegalMeta = "legal",
                WorkHash = "hash",
                WorkUri = "uri",
                WorkType = "Image",
                Protections = new Protections()
                {
                    Authorship = true,
                    CommercialAdaptation = true,
                    CommercialDistribution = true,
                    CommercialPerformance = true,
                    CommercialReproduction = true,
                    NonCommercialAdaptation = true,
                    NonCommercialDistribution = true,
                    NonCommercialPerformance = true,
                    NonCommercialReproduction = true,
                    ReviewOrCrit = true
                }
            }
        };
        var receipt = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractAddress).RegisterRequestAndWaitForReceiptAsync(funcMessage);

        receipt.Should().NotBeNull();
        receipt.Status.Value.Should().Be(1);

        var res = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractAddress).OwnershipOfQueryAsync(1);

        res.Should().NotBeNull();
        res.ReturnValue1.Select(x =>
        {
            x.Owner = x.Owner.ToLower();
            return x;
        }).First().Should().BeEquivalentTo(new OwnershipStakeContract
        {
            Owner = TestConstants.TestAccountAddress,
            Share = 100
        });
    }
}