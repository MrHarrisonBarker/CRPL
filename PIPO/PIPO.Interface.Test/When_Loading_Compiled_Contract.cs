using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PIPO.Interface.Test;

[TestFixture]
public class When_Loading_Compiled_Contract
{
    [Test]
    public async Task Should_Load_ABI_And_ByteCode()
    {
        var contract = Utils.LoadContract("Migrations");

        contract.Should().NotBeNull();
    }
}