using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.ResonanceService;

[TestFixture]
public class RegisterUser
{
    [Test]
    public async Task Should_Register()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        
        resonanceServiceFactory.ResonanceService.RegisterUser(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10"), "CONNECTION_STRING");
        
        resonanceServiceFactory.ResonanceService.UserToConnection.ContainsKey(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10")).Should().BeTrue();
        resonanceServiceFactory.ResonanceService.UserToConnection[new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10")].Should().Contain("CONNECTION_STRING");
    }
    
    [Test]
    public async Task Should_Register_To_Existing()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        resonanceServiceFactory.ResonanceService.RegisterUser(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10"), "CONNECTION_STRING");
        
        resonanceServiceFactory.ResonanceService.RegisterUser(new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10"), "CONNECTION_STRING_NEW");
        resonanceServiceFactory.ResonanceService.UserToConnection[new Guid("DB8CC4C8-D90C-49FB-8230-F450088C2D10")].Should().Contain("CONNECTION_STRING").And.Contain("CONNECTION_STRING_NEW");
    }
}