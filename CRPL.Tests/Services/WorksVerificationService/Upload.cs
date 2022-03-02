using System;
using System.IO;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.Services.WorksVerificationService;

[TestFixture]
public class Upload
{
    private IFormFile moqFile(string content = "TEST FILE CONTENT")
    {
        var fileMock = new Mock<IFormFile>();

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);

        writer.Write(content);
        writer.Flush();

        stream.Position = 0;

        fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
        fileMock.Setup(_ => _.FileName).Returns("test.fake");
        fileMock.Setup(_ => _.Length).Returns(stream.Length);
        fileMock.Setup(_ => _.ContentType).Returns("fake/data");

        return fileMock.Object;
    }

    [Test]
    public async Task Should_Upload()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        var hash = await worksVerificationServiceFactory.WorksVerificationService.Upload(moqFile());

        hash.Should().NotBeNull();

        // using sha-512 hashing algorithm so the output hash is 512 bits or 64 bytes 
        hash.Length.Should().Be(64);
    }

    [Test]
    public async Task Should_Have_Content()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await worksVerificationServiceFactory.WorksVerificationService.Upload(moqFile("")))
            .Should().ThrowAsync<Exception>().WithMessage("File needs to have content");
    }

    [Test]
    public async Task Should_Already_Exist()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        var file = moqFile();
        
        await worksVerificationServiceFactory.WorksVerificationService.Upload(file);
        
        worksVerificationServiceFactory.CachedWorkRepositoryMock.Setup(x => x.Set(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

        await FluentActions.Invoking(async () => await worksVerificationServiceFactory.WorksVerificationService.Upload(file))
            .Should().ThrowAsync<Exception>();
    }
}