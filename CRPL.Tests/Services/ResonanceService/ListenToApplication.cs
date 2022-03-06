using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using NUnit.Framework;

namespace CRPL.Tests.Services.ResonanceService;

[TestFixture]
public class ListenToApplication
{
    [Test]
    public async Task Should_Listen()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        
        resonanceServiceFactory.ResonanceService.ListenToApplication(Guid.NewGuid(), "CONNECTION_STRING");
    }
}