using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LCatalogClerk
{
    public static string LCatalogClerkFormat(LCatalogFilter? filter)
    {
        return LCatalog.LCatalogFilterFormat(filter ?? LCatalogFilter.LCatalogFilterEmpty);
    }

    public static LCatalogFilter LCatalogClerkCreate(IReadOnlyList<string> hidden)
    {
        return LCatalogFilter.LCatalogFilterCreate(hidden);
    }

    public static IReadOnlyList<LCatalogRow> LCatalogClerkFind<LCatalogRow>(
        IEnumerable<LCatalogRow> rows,
        string query,
        LCatalogOrder order,
        Func<LCatalogRow, string> key,
        Func<LCatalogRow, int> count)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(count);
        string wanted = query.Trim();

        IEnumerable<LCatalogRow> kept = rows.Where(row =>
            wanted.Length == 0 || key(row).Contains(wanted, StringComparison.OrdinalIgnoreCase));
        return order switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. kept.OrderByDescending(key, StringComparer.Ordinal)],
            LCatalogOrder.LCatalogOrderUsage => [.. kept
                .OrderByDescending(count)
                .ThenBy(key, StringComparer.Ordinal)],
            _ => [.. kept.OrderBy(key, StringComparer.Ordinal)],
        };
    }
}
