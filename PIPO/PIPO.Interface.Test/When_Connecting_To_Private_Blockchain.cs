using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Web3;
using NUnit.Framework;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Connecting_To_Private_Blockchain
{
    [Test]
    public async Task Should_Have_Account()
    {
        using var connection = TestUtils.PrivateTestConnection();
        var accounts = await connection.Web3.Eth.Accounts.SendRequestAsync();
        accounts.Length.Should().BeGreaterThan(0);
        
        Console.WriteLine($"There are {accounts.Length} accounts");
    }

    [Test]
    public async Task Account_Should_Have_Balance()
    {
        using var connection = TestUtils.PrivateTestConnection();
        var balance = await connection.Web3.Eth.GetBalance.SendRequestAsync(LawsOfNature.AccountId);

        balance.Value.Should().BeGreaterThan(0);
        
        var balanceAsEther = Web3.Convert.FromWei(balance.Value);
        
        balance.Value.Should().BeGreaterThan(0);
        
        Console.WriteLine($"The test account balance is {balance} which is {balanceAsEther} eth");
    }
}