using System;

namespace Llyn.Infrastructure;

internal static class LOutpostSeal
{
    public static bool LOutpostSealMatch(string? id)
    {
        bool valid = id is { Length: 32 };
        foreach (char letter in id ?? string.Empty)
        {
            valid &= letter is (>= '0' and <= '9') or (>= 'a' and <= 'f');
        }

        return valid;
    }

    public static void LOutpostSealCheck(string id)
    {
        if (!LOutpostSealMatch(id))
        {
            throw new ArgumentException("A Joplin id must be 32 lowercase hex characters.", nameof(id));
        }
    }
}
