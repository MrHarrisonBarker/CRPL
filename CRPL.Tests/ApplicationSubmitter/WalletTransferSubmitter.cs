using System.Threading.Tasks;
using CRPL.Tests.Factories;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationSubmitter;

[TestFixture]
public class WalletTransferSubmitter
{
    [Test]
    public async Task Should_Submit()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);
    }
}