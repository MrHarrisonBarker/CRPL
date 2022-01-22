using System;
using System.Threading.Tasks;
using CRPL.Contracts.Standard;
using CRPL.Contracts.Standard.ContractDefinition;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Blockchain;

[TestFixture]
public class When_Deploying_Standard_Contract
{
    [Test]
    public async Task Should_Deploy_Contract()
    {
        using var connection = TestConstants.PrivateTestConnection();

        var receipt = await StandardService.DeployContractAndWaitForReceiptAsync(connection.Web3, new StandardDeployment());
        
        receipt.Should().NotBeNull();
        receipt.Status.Value.Should().Be(1);
        Console.WriteLine($"Contract address -> {receipt.ContractAddress}");
    }
}