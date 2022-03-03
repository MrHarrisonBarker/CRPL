using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Account.Works;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests;

[TestFixture]
public class UsageProxyMiddleware
{
    [Test]
    public async Task Should_Proxy()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("B9BDC058-A748-457E-B886-67CF92C129C7"),
                Title = "Hello world",
                Created = DateTime.Now,
                WorkType = WorkType.Image,
                Cid = "QmSgvgwxZGaBLqkGyWemEDqikCqU52XxsYLKtdy3vGZ8uq"
            }
        });
        var serviceProviderWithContextFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var usageProxyMiddleware = new CRPL.Web.Core.UsageProxyMiddleware((innerHttpContext) => Task.CompletedTask, serviceProviderWithContextFactory.ServiceProviderMock.Object);

        DefaultHttpContext requestContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            },
            Request =
            {
                Path = $"/proxy/cpy/{new Guid("B9BDC058-A748-457E-B886-67CF92C129C7")}",
                Method = "GET"
            }
        };

        await usageProxyMiddleware.Invoke(requestContext);

        requestContext.Response.StatusCode.Should().Be(200);
        requestContext.Response.ContentType.Should().BeEquivalentTo("image/jpeg");
        requestContext.Response.Headers.Count.Should().BePositive();
        requestContext.Response.ContentLength.Should().BePositive();
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var serviceProviderWithContextFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var usageProxyMiddleware = new CRPL.Web.Core.UsageProxyMiddleware((innerHttpContext) => Task.CompletedTask, serviceProviderWithContextFactory.ServiceProviderMock.Object);
        
        DefaultHttpContext requestContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            },
            Request =
            {
                Path = $"/proxy/cpy/{Guid.Empty}",
                Method = "GET"
            }
        };

        await FluentActions.Invoking(async () => await usageProxyMiddleware.Invoke(requestContext)).Should().ThrowAsync<WorkNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_Id_Not_Found()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var serviceProviderWithContextFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var usageProxyMiddleware = new CRPL.Web.Core.UsageProxyMiddleware((innerHttpContext) => Task.CompletedTask, serviceProviderWithContextFactory.ServiceProviderMock.Object);
        
        DefaultHttpContext requestContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            },
            Request =
            {
                Path = "/proxy/cpy/",
                Method = "GET"
            }
        };

        await FluentActions.Invoking(async () => await usageProxyMiddleware.Invoke(requestContext)).Should().ThrowAsync<Exception>().WithMessage("Id not found!");
    }

    [Test]
    public async Task Should_Throw_If_Not_Guid()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var serviceProviderWithContextFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var usageProxyMiddleware = new CRPL.Web.Core.UsageProxyMiddleware((innerHttpContext) => Task.CompletedTask, serviceProviderWithContextFactory.ServiceProviderMock.Object);
        
        DefaultHttpContext requestContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            },
            Request =
            {
                Path = "/proxy/cpy/NOT-GUID",
                Method = "GET"
            }
        };

        await FluentActions.Invoking(async () => await usageProxyMiddleware.Invoke(requestContext)).Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task Should_Register_Usage()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("B9BDC058-A748-457E-B886-67CF92C129C7"),
                Title = "Hello world",
                Created = DateTime.Now,
                WorkType = WorkType.Image,
                Cid = "QmSgvgwxZGaBLqkGyWemEDqikCqU52XxsYLKtdy3vGZ8uq"
            }
        });
        var serviceProviderWithContextFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var usageProxyMiddleware = new CRPL.Web.Core.UsageProxyMiddleware((innerHttpContext) => Task.CompletedTask, serviceProviderWithContextFactory.ServiceProviderMock.Object);

        DefaultHttpContext requestContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            },
            Request =
            {
                Path = $"/proxy/cpy/{new Guid("B9BDC058-A748-457E-B886-67CF92C129C7")}",
                Method = "GET"
            }
        };

        await usageProxyMiddleware.Invoke(requestContext);
        
        serviceProviderWithContextFactory.UsageQueueMock.Verify(x => x.QueueUsage(It.IsAny<WorkUsage>()), Times.Once);
    }
}