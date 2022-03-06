using System.Threading.Tasks;
using CRPL.Tests.Factories;
using NUnit.Framework;

namespace CRPL.Tests.Services.ResonanceService;

[TestFixture]
public class RemoveConnection
{
    [Test]
    public async Task Should_Remove()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        
        resonanceServiceFactory.ResonanceService.RemoveConnection("CONNECTION_STRING");
    }
}