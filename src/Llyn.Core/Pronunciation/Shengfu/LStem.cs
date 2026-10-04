using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LStem(
    long LStemId,
    string LStemLanguage,
    string LStemKey,
    int LStemCount = 0,
    bool LStemChosen = false)
{
    public static IReadOnlyList<string> LStemKeyScan(string text, string separator)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(separator);

        string[] pieces = separator.Length == 0
            ? [text]
            : text.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        List<string> keys = [];
        foreach (string piece in pieces)
        {
            string key = piece.Trim();
            if (key.Length > 0 && !keys.Contains(key, StringComparer.Ordinal))
            {
                keys.Add(key);
            }
        }

        return keys;
    }
}
