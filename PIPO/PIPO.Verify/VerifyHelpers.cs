using System.Security.Cryptography;

namespace PIPO.Verify;

public static class VerifyHelpers
{
    public static byte[] ComputeHash(byte[] work)
    {
        using var hashAlgorithm = SHA512.Create();

        return hashAlgorithm.ComputeHash(work);
    }

    public static bool CompareWork(byte[] original, byte[] newWork)
    {
        byte[] originalHash = ComputeHash(original);
        byte[] newWorkHash = ComputeHash(newWork);

        return originalHash.SequenceEqual(newWorkHash);
    }
}