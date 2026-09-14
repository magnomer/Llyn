using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogAuthor(
    LAuthor LCatalogAuthorStored,
    int LCatalogAuthorWork,
    int LCatalogAuthorUsage)
{
    public static LCatalogAuthor LCatalogAuthorCreate(
        LAuthor author,
        IReadOnlyList<LReference>? works,
        IReadOnlyDictionary<long, int>? usage)
    {
        ArgumentNullException.ThrowIfNull(author);

        IReadOnlyList<LReference> credited = works ?? [];
        int cited = 0;
        foreach (LReference reference in credited)
        {
            if (usage is not null && usage.TryGetValue(reference.LReferenceId, out int counted))
            {
                cited += counted;
            }
        }

        return new LCatalogAuthor(author, credited.Count, cited);
    }

    public static IReadOnlyList<LCatalogAuthor> LCatalogAuthorSort(
        IReadOnlyList<LCatalogAuthor> rows,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return order switch
        {
            LCatalogOrder.LCatalogOrderWork => [.. rows
                .OrderByDescending(row => row.LCatalogAuthorWork)
                .ThenBy(row => row.LCatalogAuthorStored.LAuthorName, StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderUsage => [.. rows
                .OrderByDescending(row => row.LCatalogAuthorUsage)
                .ThenBy(row => row.LCatalogAuthorStored.LAuthorName, StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderReverse => [.. rows
                .OrderByDescending(
                    row => row.LCatalogAuthorStored.LAuthorName, StringComparer.CurrentCultureIgnoreCase)],
            _ => [.. rows
                .OrderBy(row => row.LCatalogAuthorStored.LAuthorName, StringComparer.CurrentCultureIgnoreCase)],
        };
    }

    public bool LCatalogAuthorMatch(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        return LCatalog.LCatalogTextMatch(LCatalogAuthorStored.LAuthorName, query);
    }
}
