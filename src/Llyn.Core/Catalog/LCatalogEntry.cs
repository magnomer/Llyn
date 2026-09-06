using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public static class LCatalogEntry
{
    public static IReadOnlyList<LEntry> LCatalogEntrySort(IReadOnlyList<LEntry> entries, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(entries);

        return order switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. entries
                .OrderByDescending(entry => entry.LEntryHeadword, StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderRecent => [.. entries
                .OrderByDescending(entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal)],
            LCatalogOrder.LCatalogOrderEarliest => [.. entries
                .OrderBy(entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal)],
            _ => [.. entries
                .OrderBy(entry => entry.LEntryHeadword, StringComparer.CurrentCultureIgnoreCase)],
        };
    }
}
