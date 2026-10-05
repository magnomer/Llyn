using System;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Llyn.Core;

namespace Llyn.Core.Windows;

[SupportedOSPlatform("windows")]
public sealed class LWarrantShield : LWarrant
{
    private static readonly byte[] LWarrantShieldSalt = Encoding.UTF8.GetBytes("Llyn.Warrant");

    public string LWarrantHide(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        byte[] sealedBytes = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(token), LWarrantShieldSalt, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(sealedBytes);
    }

    public string? LWarrantRestore(string hidden)
    {
        if (string.IsNullOrWhiteSpace(hidden))
        {
            return null;
        }

        try
        {
            byte[] plain = ProtectedData.Unprotect(
                Convert.FromBase64String(hidden), LWarrantShieldSalt, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plain);
        }
        catch (FormatException)
        {
            return null;
        }
        catch (CryptographicException)
        {
            return null;
        }
    }
}
