using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.BlockchainProcessing;
using NUnit.Framework;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Connecting_To_Mainnet_Blockchain
{
    [Test]
    public async Task Should_Get_Account_Balance()
    {
        using var connection = new BlockChainConnection(LawsOfNature.MainNetUrl);
        var balance = await connection.Web3.Eth.GetBalance.SendRequestAsync("0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae");

        balance.Value.Should().BeGreaterThan(0);
        
        Console.WriteLine($"The account balance is {balance.Value}");
    }
}