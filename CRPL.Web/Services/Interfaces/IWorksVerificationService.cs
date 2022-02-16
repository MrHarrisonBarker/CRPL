using CRPL.Data.Works;

namespace CRPL.Web.Services.Interfaces;

public interface IWorksVerificationService
{
    public Task VerifyWork(Guid workId);
    public Task<byte[]> Upload(IFormFile file);
    public CachedWork Sign(byte[] hash, string signature);
}