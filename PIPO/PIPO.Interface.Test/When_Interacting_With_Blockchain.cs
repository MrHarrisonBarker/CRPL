using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;
using NUnit.Framework;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Interacting_With_Blockchain
{
    [Test]
    public async Task Should_Create_Account()
    {
        var blockChainConnection = TestUtils.PrivateTestConnection();

        var accounts = await blockChainConnection.Web3.Eth.Accounts.SendRequestAsync();
        
        await blockChainConnection.Web3.Personal.NewAccount.SendRequestAsync("password");

        (await blockChainConnection.Web3.Eth.Accounts.SendRequestAsync()).Length.Should().BeGreaterThan(accounts.Length);
    }

    [Test]
    public async Task Should_Get_Balance()
    {
        var account = new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId);
        var blockChainConnection = new BlockChainConnection(LawsOfNature.ChainUrl, account);

        var balance = await blockChainConnection.Web3.Eth.GetBalance.SendRequestAsync(account.Address);

        balance.Value.Should().BeGreaterThan(0);
    }
}