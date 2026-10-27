using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Llyn.Core;

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

    public static string LOutpostSealFormat(string seed)
    {
        ArgumentNullException.ThrowIfNull(seed);
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(seed)))[..32];
    }

    public static string LOutpostSealFormat(
        LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(parcels);

        StringBuilder text = new StringBuilder();
        text.Append(note.LOutpostNoteTitle).Append('\0')
            .Append(note.LOutpostNoteFolder).Append('\0')
            .Append(note.LOutpostNoteBody).Append('\0');
        foreach (string tag in tags.Order(StringComparer.Ordinal))
        {
            text.Append(tag).Append('\u0001');
        }

        text.Append('\0');
        foreach (LParcel parcel in parcels)
        {
            text.Append(parcel.LParcelId).Append('\u0001');
        }

        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));
    }
}
