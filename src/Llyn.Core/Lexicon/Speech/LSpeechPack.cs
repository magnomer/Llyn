using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LSpeechPack(
    IReadOnlyList<LSpeechValue> LSpeechPackValues,
    IReadOnlyList<LFeature> LSpeechPackFeatures,
    IReadOnlyList<LMorphology> LSpeechPackMorphology,
    IReadOnlyList<LParadigm> LSpeechPackParadigms,
    IReadOnlyList<LSpeechRetirement>? LSpeechPackRetirements = null)
{
    public IReadOnlyList<LSpeechRetirement> LSpeechPackRetirements { get; init; } = LSpeechPackRetirements ?? [];

    public IReadOnlyList<LSpeechValue> LSpeechPackSort(IReadOnlyList<LSpeechValue> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .OrderBy(row => LSpeechPackFind(row.LSpeechValueCode))
            .ThenBy(static row => row.LSpeechValueName, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(static row => row.LSpeechValueName, StringComparer.Ordinal)
            .ThenBy(static row => row.LSpeechValueId)
            .ToList();
    }

    private int LSpeechPackFind(long code)
    {
        if (code <= 0)
        {
            return int.MaxValue;
        }

        for (int index = 0; index < LSpeechPackValues.Count; index++)
        {
            if (LSpeechPackValues[index].LSpeechValueCode == code)
            {
                return index;
            }
        }

        return int.MaxValue;
    }
}
