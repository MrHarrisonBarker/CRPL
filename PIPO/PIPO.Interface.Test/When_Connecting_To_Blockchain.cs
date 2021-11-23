using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NUnit.Framework;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Connecting_To_Blockchain
{
    [Test]
    public async Task Should_Have_Account()
    {
        var web3 = new Web3(new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId), LawsOfNature.ChainUrl);
        var accounts = await web3.Eth.Accounts.SendRequestAsync();
        accounts.Length.Should().BeGreaterThan(0);
    }
}