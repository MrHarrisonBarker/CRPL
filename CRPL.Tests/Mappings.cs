using AutoMapper;
using CRPL.Data;
using NUnit.Framework;

namespace CRPL.Tests;

[TestFixture]
public class Mappings
{
    [Test]
    public void Map_Should_HaveValidConfig()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
        config.AssertConfigurationIsValid();
    }
}