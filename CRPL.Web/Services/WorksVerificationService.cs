using System.Security.Cryptography;
using CRPL.Data.Account;
using CRPL.Data.Workds;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

public class WorksVerificationService : IWorksVerificationService
{
    private readonly ILogger<WorksVerificationService> Logger;
    private readonly ApplicationContext Context;

    public WorksVerificationService(ILogger<WorksVerificationService> logger, ApplicationContext context)
    {
        Logger = logger;
        Context = context;
    }

    public async Task<VerificationResult> VerifyWork(byte[] hash)
    {
        var collisions = await Context.RegisteredWorks.Where(x => x.Hash == hash).Select(x => x.RightId).ToListAsync();
        return new VerificationResult()
        {
            IsAuthentic = collisions.Count == 0,
            Collisions = collisions
        };
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