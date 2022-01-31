using CRPL.Data.Workds;
using CRPL.Data.Works;

namespace CRPL.Web.Services.Interfaces;

public interface IWorksVerificationService
{
    public Task<VerificationResult> VerifyWork(byte[] hash);
    public Task<byte[]> Upload(IFormFile file);
    public CachedWork Sign(byte[] hash, string signature);
}