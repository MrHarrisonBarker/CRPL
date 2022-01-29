using CRPL.Data.Workds;

namespace CRPL.Web.Services.Interfaces;

public interface IWorksVerificationService
{
    public Task<VerificationResult> VerifyWork(byte[] raw);
    public Task<byte[]> Upload(IFormFile file);
}