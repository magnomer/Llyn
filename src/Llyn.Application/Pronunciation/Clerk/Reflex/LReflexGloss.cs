using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReflexGloss
{
    private readonly Dictionary<long, Dictionary<string, string>> _lReflexGlossHeld = [];

    public void LReflexGlossRecord(long entryId, IReadOnlyList<LReflex> rows)
    {
        Dictionary<string, string> held = [];
        foreach (LReflex row in rows)
        {
            if (row.LReflexOwned && row.LReflexMeaning.Length > 0)
            {
                held[LReflexGlossFormat(row)] = row.LReflexMeaning;
            }
        }

        if (held.Count > 0)
        {
            _lReflexGlossHeld[entryId] = held;
        }
        else
        {
            _lReflexGlossHeld.Remove(entryId);
        }
    }

    public IReadOnlyList<LReflex> LReflexGlossRestore(long entryId, IReadOnlyList<LReflex> rows)
    {
        if (!_lReflexGlossHeld.Remove(entryId, out Dictionary<string, string>? held))
        {
            return rows;
        }

        List<LReflex> filled = new(rows.Count);
        foreach (LReflex row in rows)
        {
            filled.Add(held.TryGetValue(LReflexGlossFormat(row), out string? meaning)
                ? row with { LReflexMeaning = meaning, LReflexOwned = true }
                : row);
        }

        return filled;
    }

    public void LReflexGlossClear()
    {
        _lReflexGlossHeld.Clear();
    }

    private static string LReflexGlossFormat(LReflex row)
    {
        return string.Join(
            '\u001F', row.LReflexLanguage, row.LReflexRegion, row.LReflexKind, row.LReflexText);
    }
}
