using System;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.ResonanceService;

[TestFixture]
public class RemoveConnection
{
    [Test]
    public async Task Should_Remove()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        
        resonanceServiceFactory.ResonanceService.ListenToWork(Guid.NewGuid(), "CONNECTION_STRING");
        resonanceServiceFactory.ResonanceService.ListenToApplication(Guid.NewGuid(), "CONNECTION_STRING");
        resonanceServiceFactory.ResonanceService.RegisterUser(Guid.NewGuid(), "CONNECTION_STRING");
        
        resonanceServiceFactory.ResonanceService.RemoveConnection("CONNECTION_STRING");

        resonanceServiceFactory.ResonanceService.ApplicationToConnection.Values.Should().NotContain(list => list.Any(x => x == "CONNECTION_STRING"));
        resonanceServiceFactory.ResonanceService.UserToConnection.Values.Should().NotContain(list => list.Any(x => x == "CONNECTION_STRING"));
        resonanceServiceFactory.ResonanceService.WorkToConnection.Values.Should().NotContain(list => list.Any(x => x == "CONNECTION_STRING"));
    }
}