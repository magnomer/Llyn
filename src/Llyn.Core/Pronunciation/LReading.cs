using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Llyn.Core;

public sealed record LReading(string LReadingVariety, string LReadingPhonetic)
{
    private const string LReadingBoundaries = ".\u00B7\u2027";

    public static string LReadingNormalize(string phonetic)
    {
        string composed = phonetic.Normalize(NormalizationForm.FormC);
        StringBuilder kept = new(composed.Length);
        foreach (char symbol in composed)
        {
            if (!LReadingBoundaries.Contains(symbol) && !char.IsWhiteSpace(symbol))
            {
                kept.Append(symbol);
            }
        }

        return kept.ToString().Trim();
    }

    public static IReadOnlyList<LReading> LReadingScan(
        IReadOnlyList<LReading> readings,
        IReadOnlyList<LVariety> varieties)
    {
        ArgumentNullException.ThrowIfNull(readings);
        ArgumentNullException.ThrowIfNull(varieties);

        if (varieties.Count == 0)
        {
            return readings;
        }

        List<LReading> tagged = readings.Where(static reading => reading.LReadingVariety.Length > 0).ToList();
        List<LReading> expanded = new(tagged);
        foreach (LReading reading in readings)
        {
            if (reading.LReadingVariety.Length > 0)
            {
                continue;
            }

            foreach (LVariety variety in varieties)
            {
                if (tagged.All(known => !string.Equals(known.LReadingVariety, variety.LVarietyName, StringComparison.Ordinal)))
                {
                    expanded.Add(reading with { LReadingVariety = variety.LVarietyName });
                }
            }
        }

        return expanded;
    }
}
