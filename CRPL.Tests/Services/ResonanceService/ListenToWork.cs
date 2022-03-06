using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using NUnit.Framework;

namespace CRPL.Tests.Services.ResonanceService;

[TestFixture]
public class ListenToWork
{
    [Test]
    public async Task Should_Listen()
    {
        var resonanceServiceFactory = new ResonanceServiceFactory();
        
        resonanceServiceFactory.ResonanceService.ListenToWork(Guid.NewGuid(), "CONNECTION_STRING");
    }
}