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
    public bool LStemMatch(long id)
    {
        return LStemId == id;
    }

    public bool LStemMatch(LStem other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return LStemId == other.LStemId
            && string.Equals(LStemKey, other.LStemKey, StringComparison.Ordinal)
            && LStemCount == other.LStemCount;
    }

    public static LStem? LStemFind(IReadOnlyList<LStem> rows, long id)
    {
        ArgumentNullException.ThrowIfNull(rows);

        foreach (LStem row in rows)
        {
            if (row.LStemMatch(id))
            {
                return row;
            }
        }

        return null;
    }

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
