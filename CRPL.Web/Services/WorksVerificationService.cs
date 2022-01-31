using System.Security.Cryptography;
using CRPL.Data.Account;
using CRPL.Data.Workds;
using CRPL.Data.Works;
using CRPL.Web.Services.Interfaces;
using CRPL.Web.WorkSigners;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

public class WorksVerificationService : IWorksVerificationService
{
    private readonly ILogger<WorksVerificationService> Logger;
    private readonly ApplicationContext Context;
    private readonly ICachedWorkRepository CachedWorkRepository;

    public WorksVerificationService(ILogger<WorksVerificationService> logger, ApplicationContext context, ICachedWorkRepository cachedWorkRepository)
    {
        Logger = logger;
        Context = context;
        CachedWorkRepository = cachedWorkRepository;
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

        var work = stream.GetBuffer();

        var hash = HashWork(work);

        CachedWorkRepository.Set(hash, work, file.ContentType);

        Logger.LogInformation("hashed {Name} into {Hash}", file.FileName, hash);

        return hash;
    }

    public CachedWork Sign(byte[] hash)
    {
        Logger.LogInformation("Signing work {Hash}", hash);

        var work = CachedWorkRepository.Get(hash);

        switch (work.ContentType)
        {
            case var type when type.Contains("image"):
                work = new ImageSigner().Sign(work);
                break;
        }

        return new UniversalSigner().Sign(work);
    }

    private byte[] HashWork(byte[] work)
    {
        using var hashAlgorithm = SHA512.Create();

        return hashAlgorithm.ComputeHash(work);
    }
}