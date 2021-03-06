using CRPL.Data.Account;

namespace CRPL.Web.Services.Interfaces;

public interface IWorksVerificationService
{
    public Task VerifyWork(Guid workId);
    public Task<byte[]> Upload(IFormFile file);
    public Task<RegisteredWork> Sign(RegisteredWork work);
}