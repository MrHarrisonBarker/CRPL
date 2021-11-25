using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Connecting_To_Ropsten_Test_Blockchain
{
    [Test]
    public async Task Should_Get_Account_Balance()
    {
        using var connection = TestUtils.RopstenTestConnection();
        
        // get the balance of my metamask test account
        var balance = await connection.Web3.Eth.GetBalance.SendRequestAsync("0x3aaf677ea4e72eebb92d2d5c3a92307ee789e24c");

        balance.Value.Should().BeGreaterThan(0);
        
        Console.WriteLine($"The account balance is {balance.Value}");
    }
}