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
            LCatalogOrder.LCatalogOrderReverse => [.. LCatalogEntry.LCatalogEntrySort(
                favorites.OrderByDescending(
                    favorite => favorite.LCatalogFavoriteEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase),
                static favorite => favorite.LCatalogFavoriteEntry)],
            LCatalogOrder.LCatalogOrderLanguage => [.. LCatalogEntry.LCatalogEntrySort(
                favorites.OrderBy(
                    favorite => favorite.LCatalogFavoriteEntry.LEntryLanguage,
                    StringComparer.CurrentCultureIgnoreCase),
                static favorite => favorite.LCatalogFavoriteEntry)],
            LCatalogOrder.LCatalogOrderMarked => [.. LCatalogEntry.LCatalogEntrySort(
                favorites.OrderByDescending(favorite => favorite.LCatalogFavoriteMarked, StringComparer.Ordinal),
                static favorite => favorite.LCatalogFavoriteEntry)],
            LCatalogOrder.LCatalogOrderGrasp => [.. LCatalogEntry.LCatalogEntrySort(
                favorites.OrderByDescending(favorite => favorite.LCatalogFavoriteEntry.LEntryGrasp)
                    .ThenBy(
                        favorite => favorite.LCatalogFavoriteEntry.LEntryHeadword,
                        StringComparer.CurrentCultureIgnoreCase),
                static favorite => favorite.LCatalogFavoriteEntry)],
            _ => [.. LCatalogEntry.LCatalogEntrySort(
                favorites.OrderBy(
                    favorite => favorite.LCatalogFavoriteEntry.LEntryHeadword,
                    StringComparer.CurrentCultureIgnoreCase),
                static favorite => favorite.LCatalogFavoriteEntry)],
        };
    }
}
