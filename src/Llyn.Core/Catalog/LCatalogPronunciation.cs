using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogPronunciation(
    LEntry LCatalogPronunciationEntry,
    string LCatalogPronunciationSound)
{
    public static LCatalogPronunciation LCatalogPronunciationCreate(LEntry entry, string? sound)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new LCatalogPronunciation(entry, sound ?? string.Empty);
    }

    public static IReadOnlyList<LCatalogPronunciation> LCatalogPronunciationSort(
        IReadOnlyList<LCatalogPronunciation> rows,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return order switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. rows
                .OrderByDescending(
                    row => row.LCatalogPronunciationEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderSound => [.. rows
                .OrderBy(row => row.LCatalogPronunciationSound.Length == 0)
                .ThenBy(row => row.LCatalogPronunciationSound, StringComparer.Ordinal)],
            LCatalogOrder.LCatalogOrderPending => [.. rows
                .OrderByDescending(row => row.LCatalogPronunciationSound.Length == 0)
                .ThenBy(
                    row => row.LCatalogPronunciationEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
            _ => [.. rows
                .OrderBy(
                    row => row.LCatalogPronunciationEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
        };
    }
}
