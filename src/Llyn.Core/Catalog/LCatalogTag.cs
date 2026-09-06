using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public static class LCatalogTag
{
    public static IReadOnlyList<LTag> LCatalogTagSort(IReadOnlyList<LTag> tags, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(tags);

        return order switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. tags
                .OrderByDescending(tag => tag.LTagText, StringComparer.CurrentCultureIgnoreCase)],
            _ => [.. tags
                .OrderBy(tag => tag.LTagText, StringComparer.CurrentCultureIgnoreCase)],
        };
    }

    public static bool LCatalogTagMatch(LTag tag, string query)
    {
        ArgumentNullException.ThrowIfNull(tag);

        return LCatalog.LCatalogTextMatch(tag.LTagText, query);
    }
}
