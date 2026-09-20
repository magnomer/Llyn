using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogFavorite(LEntry LCatalogFavoriteEntry, string LCatalogFavoriteMarked)
{
    public static IReadOnlyList<LCatalogFavorite> LCatalogFavoriteSort(
        IReadOnlyList<LCatalogFavorite> favorites,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(favorites);

        return order switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. favorites
                .OrderByDescending(
                    favorite => favorite.LCatalogFavoriteEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderLanguage => [.. favorites
                .OrderBy(
                    favorite => favorite.LCatalogFavoriteEntry.LEntryLanguage,
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderMarked => [.. favorites
                .OrderByDescending(favorite => favorite.LCatalogFavoriteMarked, StringComparer.Ordinal)],
            LCatalogOrder.LCatalogOrderGrasp => [.. favorites
                .OrderByDescending(favorite => favorite.LCatalogFavoriteEntry.LEntryGrasp)
                .ThenBy(
                    favorite => favorite.LCatalogFavoriteEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
            _ => [.. favorites
                .OrderBy(
                    favorite => favorite.LCatalogFavoriteEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase)],
        };
    }
}
