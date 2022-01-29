using System.Security.Cryptography;
using CRPL.Data.Workds;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

class WorksVerificationService : IWorksVerificationService
{
    private readonly ILogger<WorksVerificationService> Logger;

    public WorksVerificationService(ILogger<WorksVerificationService> logger)
    {
        Logger = logger;
    }

    public Task<VerificationResult> VerifyWork(byte[] raw)
    {
        throw new NotImplementedException();
    }

    public async Task<byte[]> Upload(IFormFile file)
    {
        Logger.LogInformation("Uploading file {FileName}", file.FileName);

        await using var stream = new MemoryStream();

        await file.CopyToAsync(stream);

        Logger.LogInformation("Uploaded file of length {Length}", stream.Length);

        var hash = await HashWork(stream);

        Logger.LogInformation("hashed {Name} into {Hash}", file.FileName, hash);

        return hash;
    }

    private async Task<byte[]> HashWork(Stream stream)
    {
        using var hashAlgorithm = SHA512.Create();

        return await hashAlgorithm.ComputeHashAsync(stream);
    }
}