using System;
using System.Threading.Tasks;
using CRPL.Data;
using FluentAssertions;
using Nethereum.Web3;
using NUnit.Framework;

namespace CRPL.Tests.Blockchain;

[TestFixture]
public class When_Connecting_To_Private_Blockchain
{
    [Test]
    [Ignore("need ci blockchain")]
    public async Task Should_Have_Account()
    {
        using var connection = TestConstants.PrivateTestConnection();
        var accounts = await connection.Web3.Eth.Accounts.SendRequestAsync();
        accounts.Length.Should().BeGreaterThan(0);
        
        Console.WriteLine($"There are {accounts.Length} accounts");
    }

    [Test]
    [Ignore("need ci blockchain")]
    public async Task Account_Should_Have_Balance()
    {
        using var connection = TestConstants.PrivateTestConnection();
        var balance = await connection.Web3.Eth.GetBalance.SendRequestAsync(TestConstants.TestAccountId);

        balance.Value.Should().BeGreaterThan(0);
        
        var balanceAsEther = Web3.Convert.FromWei(balance.Value);
        
        balance.Value.Should().BeGreaterThan(0);
        
        Console.WriteLine($"The test account balance is {balance} which is {balanceAsEther} eth");
    }
}