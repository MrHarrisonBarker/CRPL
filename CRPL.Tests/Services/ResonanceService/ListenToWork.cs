using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.ResonanceService;

[TestFixture]
public class ListenToWork
{
    [Test]
    public async Task Should_Listen()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        
        resonanceServiceFactory.ResonanceService.ListenToWork(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10"), "CONNECTION_STRING");

        resonanceServiceFactory.ResonanceService.WorkToConnection.ContainsKey(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10")).Should().BeTrue();
        resonanceServiceFactory.ResonanceService.WorkToConnection[new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10")].Should().Contain("CONNECTION_STRING");
    }

    [Test]
    public async Task Should_Listen_To_Existing()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        resonanceServiceFactory.ResonanceService.ListenToWork(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10"), "CONNECTION_STRING");
        
        resonanceServiceFactory.ResonanceService.ListenToWork(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10"), "CONNECTION_STRING_NEW");
        resonanceServiceFactory.ResonanceService.WorkToConnection[new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10")].Should().Contain("CONNECTION_STRING").And.Contain("CONNECTION_STRING_NEW");
    }
    
}