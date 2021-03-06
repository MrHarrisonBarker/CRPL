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

// A service for verifying and signing works
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

    // Find collisions by comparing work hashes
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

        // If collision found update work and appliaction status
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

        // Save work id of collision 
        work.VerificationResult = new VerificationResult
        {
            Collision = collision?.Id,
            IsAuthentic = collision == null
        };

        await Context.SaveChangesAsync();
        
        // Push work and application updates to websockets
        await ResonanceService.PushWorkUpdates(work);
        await ResonanceService.PushApplicationUpdates(application);
    }

    // Upload a digital work for signing and distribution
    public async Task<byte[]> Upload(IFormFile file)
    {
        Logger.LogInformation("Uploading file {FileName}", file.FileName);

        if (file.Length == 0) throw new Exception("File needs to have content");

        await using var stream = new MemoryStream();
        
        await file.CopyToAsync(stream);

        Logger.LogInformation("Uploaded file of length {Length}", stream.Length);

        // Stream file into byte array
        var work = stream.GetBuffer();

        var hash = HashWork(work);

        // Save work in the cached work repository
        CachedWorkRepository.Set(hash, work, file.ContentType, file.FileName);

        Logger.LogInformation("hashed {Name} into {Hash}", file.FileName, hash);

        return hash;
    }

    // Digitally sign an uploaded work
    public async Task<RegisteredWork> Sign(RegisteredWork work)
    {
        Logger.LogInformation("Signing work for {Id}", work.Id);

        if (work.Status != RegisteredWorkStatus.Registered || !work.Registered.HasValue) throw new WorkNotRegisteredException();

        Logger.LogInformation("Signing work {Hash}", work.Hash);

        // Signature to insert into the file
        var signature =
            $"CRPL COPYRIGHT SIGNATURE ({work.Registered.Value.ToLongDateString()} at {work.Registered.Value.ToLongTimeString()}) // {work.Id} // registered on the chain at {work.RegisteredTransactionId} // right id {work.RightId}";

        var cachedWork = CachedWorkRepository.Get(work.Hash);

        // Sign a file based on file type
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

        // Sign every file type using the universal signer
        var signedWork = new UniversalSigner(signature).Sign(cachedWork);
        
        // Add the file to IPFS and save CID to database
        var cid = await IpfsConnection.AddFile(new MemoryStream(signedWork.Work), signedWork.FileName);

        work.Cid = cid.ToString();
        Logger.LogInformation("Uploaded work to ipfs at: {Id}", work.Cid);

        return work;
    }

    // Hash a file using dotnet's SHA512 builtin algorithm
    private byte[] HashWork(byte[] work)
    {
        using var hashAlgorithm = SHA512.Create();

        return hashAlgorithm.ComputeHash(work);
    }
}