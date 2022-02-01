using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data;
using CRPL.Data.BlockchainUtils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using NUnit.Framework;

namespace CRPL.Tests.Blockchain;

[TestFixture]
public class When_Connecting_To_Private_Blockchain
{
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
    }

    [Test]
    [Ignore("need ci blockchain")]
    public async Task Should_Have_Account()
    {
        var accounts = await BlockchainConnection.Web3().Eth.Accounts.SendRequestAsync();
        accounts.Length.Should().BeGreaterThan(0);
        
        Console.WriteLine($"There are {accounts.Length} accounts");
    }

    [Test]
    [Ignore("need ci blockchain")]
    public async Task Account_Should_Have_Balance()
    {
        var balance = await BlockchainConnection.Web3().Eth.GetBalance.SendRequestAsync(TestConstants.TestAccountAddress);

        balance.Value.Should().BeGreaterThan(0);
        
        var balanceAsEther = Web3.Convert.FromWei(balance.Value);
        
        balance.Value.Should().BeGreaterThan(0);
        
        Console.WriteLine($"The test account balance is {balance} which is {balanceAsEther} eth");
    }
}