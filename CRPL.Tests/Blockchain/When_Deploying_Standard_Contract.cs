using CRPL.Data.BlockchainUtils;
using NUnit.Framework;

namespace CRPL.Tests.Blockchain;

[TestFixture]
public class When_Deploying_Standard_Contract
{
    private BlockchainConnection BlockchainConnection;
    
    // [SetUp]
    // [Ignore("need ci blockchain")]
    // public async Task SetUp()
    // {
    //     var appSettings = Options.Create(new AppSettings()
    //     {
    //         Chains = new List<Chain>()
    //         {
    //             new()
    //             {
    //                 Name = "LOCAL",
    //                 Url = "http://localhost:8545",
    //                 Id = "444444444500"
    //             }
    //         },
    //         SystemAccount = new SystemAccount()
    //         {
    //             AccountId = TestConstants.TestAccountAddress,
    //             PrivateKey = TestConstants.TestAccountPrivateKey
    //         }
    //     });
    //     
    //     Environment.SetEnvironmentVariable("CURRENT_CHAIN","LOCAL");
    //     
    //     BlockchainConnection = new BlockchainConnection(new Logger<BlockchainConnection>(new LoggerFactory()), appSettings);
    // }
    //
    // [Test]
    // // [Ignore("need ci blockchain")]
    // public async Task Should_Deploy_Contract()
    // {
    //     var receipt = await StandardService.DeployContractAndWaitForReceiptAsync(BlockchainConnection.Web3(), new StandardDeployment());
    //     
    //     receipt.Should().NotBeNull();
    //     receipt.Status.Value.Should().Be(1);
    //     Console.WriteLine($"Contract address -> {receipt.ContractAddress}");
    // }
}