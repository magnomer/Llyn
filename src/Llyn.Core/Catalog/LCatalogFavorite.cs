using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public static class LCatalogFavorite
{
    public static IReadOnlyList<LFavorite> LCatalogFavoriteSort(
        IReadOnlyList<LFavorite> favorites,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(favorites);

        return order switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. favorites
                .OrderByDescending(
                    favorite => favorite.LFavoriteEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderLanguage => [.. favorites
                .OrderBy(
                    favorite => favorite.LFavoriteEntry.LEntryLanguage,
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderMarked => [.. favorites
                .OrderByDescending(favorite => favorite.LFavoriteMarkedUtc, StringComparer.Ordinal)],
            _ => [.. favorites
                .OrderBy(
                    favorite => favorite.LFavoriteEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
        };
    }
}
