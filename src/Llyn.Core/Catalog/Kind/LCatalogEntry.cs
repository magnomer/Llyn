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
            LCatalogOrder.LCatalogOrderReverse => [.. LCatalogEntrySort(
                entries.OrderByDescending(entry => entry.LEntryHeadword, StringComparer.CurrentCultureIgnoreCase),
                static entry => entry)],
            LCatalogOrder.LCatalogOrderRecent => [.. LCatalogEntrySort(
                entries.OrderByDescending(entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal),
                static entry => entry)],
            LCatalogOrder.LCatalogOrderEarliest => [.. LCatalogEntrySort(
                entries.OrderBy(entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal),
                static entry => entry)],
            _ => [.. LCatalogEntrySort(
                entries.OrderBy(entry => entry.LEntryHeadword, StringComparer.CurrentCultureIgnoreCase),
                static entry => entry)],
        };
    }

    public static IOrderedEnumerable<LCatalogEntryRow> LCatalogEntrySort<LCatalogEntryRow>(
        IOrderedEnumerable<LCatalogEntryRow> rows,
        Func<LCatalogEntryRow, LEntry> entry)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(entry);

        return rows
            .ThenBy(row => entry(row).LEntryLanguage, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(row => entry(row).LEntryHeadword, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(row => entry(row).LEntryId);
    }
}
