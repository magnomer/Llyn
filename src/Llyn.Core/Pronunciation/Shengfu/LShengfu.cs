using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LShengfu(
    string LShengfuCharacter,
    string LShengfuText,
    string LShengfuSource = "")
{
    public string LShengfuSource { get; init; } = LShengfuSource ?? string.Empty;

    public bool LShengfuWritten => LShengfuText.Length > 0;

    public static string LShengfuTextFind(IReadOnlyList<LShengfu> rows, string character)
    {
        ArgumentNullException.ThrowIfNull(rows);

        foreach (LShengfu row in rows)
        {
            if (string.Equals(row.LShengfuCharacter, character, StringComparison.Ordinal))
            {
                return row.LShengfuText;
            }
        }

        return string.Empty;
    }
}
