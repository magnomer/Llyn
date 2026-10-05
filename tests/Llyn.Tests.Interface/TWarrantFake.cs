using System;
using Llyn.Core;

namespace Llyn.Tests;

public sealed class TWarrantFake : LWarrant
{
    private const string TWarrantFakeMark = "fake:";

    public string LWarrantHide(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        return TWarrantFakeMark + TWarrantFakeFormat(token);
    }

    public string? LWarrantRestore(string hidden)
    {
        if (string.IsNullOrWhiteSpace(hidden) || !hidden.StartsWith(TWarrantFakeMark, StringComparison.Ordinal))
        {
            return null;
        }

        return TWarrantFakeFormat(hidden.Substring(TWarrantFakeMark.Length));
    }

    private static string TWarrantFakeFormat(string text)
    {
        char[] letters = text.ToCharArray();
        Array.Reverse(letters);
        return new string(letters);
    }
}
