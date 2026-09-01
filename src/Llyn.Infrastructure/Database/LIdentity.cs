using System.Security.Cryptography;

namespace Llyn.Infrastructure;

public static class LIdentity
{
    private const int LIdentityLength = 12;
    private const string LIdentityAlphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

    public static string LIdentityCreate()
    {
        return RandomNumberGenerator.GetString(LIdentityAlphabet, LIdentityLength);
    }
}
