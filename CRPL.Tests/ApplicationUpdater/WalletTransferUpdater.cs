using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using CRPL.Web.Services;
using FluentAssertions;
using Moq;
using Nethereum.Hex.HexTypes;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationUpdater;

[TestFixture]
public class WalletTransferUpdater
{
    [Test]
    public async Task Should_Update()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new WalletTransferApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context, MockWebUtils.FromDefault(new Dictionary<string, object>()
        {
            {"eth_getBalance", new HexBigInteger(BigInteger.One)}
        }));

        var updatedApplication = (WalletTransferApplication)await dbFactory.Context.Applications.First().UpdateApplication(new WalletTransferInputModel
        {
            Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
            UserId = new Guid("8729B942-B8A4-46D3-BCCB-9997C865FF20"),
            WalletAddress = "ADDRESS"
        }, serviceProviderFactory.ServiceProviderMock.Object);

        updatedApplication.WalletAddress.Should().BeEquivalentTo("ADDRESS");

        serviceProviderFactory.UserServiceMock.Verify(x => x.AssignToApplication(new Guid("8729B942-B8A4-46D3-BCCB-9997C865FF20"), new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF")), Times.Once);
    }

    // TODO
    // [Test]
    // public async Task Should_Throw_When_No_Balance()
    // {
    //     using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
    //     {
    //         new WalletTransferApplication
    //         {
    //             Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
    //             Created = DateTime.Now,
    //             Modified = DateTime.Now.AddDays(-1)
    //         }
    //     });
    //     var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context, MockWebUtils.FromDefault(new Dictionary<string, object>()
    //     {
    //         {"eth_getBalance", new HexBigInteger("")}
    //     }));
    //
    //     await FluentActions.Invoking(async () => await dbFactory.Context.Applications.First().UpdateApplication(new WalletTransferInputModel
    //     {
    //         Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
    //         UserId = new Guid("8729B942-B8A4-46D3-BCCB-9997C865FF20"),
    //         WalletAddress = "ADDRESS"
    //     }, serviceProviderFactory.ServiceProviderMock.Object)).Should().ThrowAsync<WalletNotFoundException>();
    // }
}