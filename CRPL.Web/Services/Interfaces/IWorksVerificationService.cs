using CRPL.Data.Works;

namespace CRPL.Web.Services.Interfaces;

public interface IWorksVerificationService
{
    public Task VerifyWork(Guid workId);
    public Task<byte[]> Upload(IFormFile file);
    public Task<CachedWork> Sign(Guid workId);
    public Task<CachedWork> GetSigned(Guid workId);
}