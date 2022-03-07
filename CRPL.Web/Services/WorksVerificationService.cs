using System.Security.Cryptography;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.Workds;
using CRPL.Data.Works;
using CRPL.Web.Core;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using CRPL.Web.WorkSigners;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRPL.Web.Services;

public class WorksVerificationService : IWorksVerificationService
{
    private readonly ILogger<WorksVerificationService> Logger;
    private readonly ApplicationContext Context;
    private readonly ICachedWorkRepository CachedWorkRepository;
    private readonly IIpfsConnection IpfsConnection;
    private readonly IResonanceService ResonanceService;
    private readonly AppSettings AppSettings;

    public WorksVerificationService(
        ILogger<WorksVerificationService> logger,
        ApplicationContext context,
        IOptions<AppSettings> appSettings,
        ICachedWorkRepository cachedWorkRepository,
        IIpfsConnection ipfsConnection,
        IResonanceService resonanceService)
    {
        Logger = logger;
        Context = context;
        CachedWorkRepository = cachedWorkRepository;
        IpfsConnection = ipfsConnection;
        ResonanceService = resonanceService;
        AppSettings = appSettings.Value;
    }

    public async Task VerifyWork(Guid workId)
    {
        Logger.LogInformation("Verifying {Id}", workId);

        var work = await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .FirstOrDefaultAsync(x => x.Id == workId);
        if (work == null) throw new WorkNotFoundException(workId);

        var application = work.AssociatedApplication.FirstOrDefault(x => x.ApplicationType == ApplicationType.CopyrightRegistration);
        if (application == null) throw new ApplicationNotFoundException();

        Context.Update(work);
        Context.Update(application);

        var collision = await Context.RegisteredWorks
            .FirstOrDefaultAsync(x => x.Hash == work.Hash && x.Id != workId);

        if (collision != null)
        {
            Logger.LogInformation("Found a collision when verifying work {Id}", workId);
            work.Status = RegisteredWorkStatus.Rejected;
            application.Status = ApplicationStatus.Failed;
        }
        else
        {
            Logger.LogInformation("Found no collision when verifying work {Id}", workId);
            work.Status = RegisteredWorkStatus.Verified;
        }

        work.VerificationResult = new VerificationResult
        {
            Collision = collision?.Id,
            IsAuthentic = collision == null
        };

        await Context.SaveChangesAsync();
        
        await ResonanceService.PushWorkUpdates(work);
        await ResonanceService.PushApplicationUpdates(application);
    }

    public async Task<byte[]> Upload(IFormFile file)
    {
        Logger.LogInformation("Uploading file {FileName}", file.FileName);

        if (file.Length == 0) throw new Exception("File needs to have content");

        await using var stream = new MemoryStream();

        await file.CopyToAsync(stream);

        Logger.LogInformation("Uploaded file of length {Length}", stream.Length);

        var work = stream.GetBuffer();

        var hash = HashWork(work);

        CachedWorkRepository.Set(hash, work, file.ContentType, file.FileName);

        Logger.LogInformation("hashed {Name} into {Hash}", file.FileName, hash);

        return hash;
    }

    public async Task<RegisteredWork> Sign(RegisteredWork work)
    {
        Logger.LogInformation("Signing work for {Id}", work.Id);

        if (work.Status != RegisteredWorkStatus.Registered || !work.Registered.HasValue) throw new WorkNotRegisteredException();

        Logger.LogInformation("Signing work {Hash}", work.Hash);

        var signature =
            $"CRPL COPYRIGHT SIGNATURE ({work.Registered.Value.ToLongDateString()} at {work.Registered.Value.ToLongTimeString()}) // {work.Id} // registered on the chain at {work.RegisteredTransactionId} // right id {work.RightId}";

        var cachedWork = CachedWorkRepository.Get(work.Hash);

        switch (cachedWork.ContentType)
        {
            case var type when type.Contains("image"):
                cachedWork = new ImageSigner(signature).Sign(cachedWork);
                break;
            case "application/pdf":
                cachedWork = new TextSigner(signature).Sign(cachedWork);
                break;
            case "audio/mpeg":
                cachedWork = new SoundSigner(signature).Sign(cachedWork);
                break;
            case var type when type.Contains("video"):
                cachedWork = new VideoSigner(signature).Sign(cachedWork);
                break;
        }

        var signedWork = new UniversalSigner(signature).Sign(cachedWork);
        
        var cid = await IpfsConnection.AddFile(new MemoryStream(signedWork.Work), signedWork.FileName);

        work.Cid = cid.ToString();
        Logger.LogInformation("Uploaded work to ipfs at: {Id}", work.Cid);

        return work;
    }

    private byte[] HashWork(byte[] work)
    {
        using var hashAlgorithm = SHA512.Create();

        return hashAlgorithm.ComputeHash(work);
    }
}