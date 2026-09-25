using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LFavoriteClerk
{
    private readonly LFavoriteVault _lFavoriteClerkFavorites;

    public LFavoriteClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lFavoriteClerkFavorites = rig.LRigFavorites;
    }

    public IReadOnlyList<LCatalogFavorite> LFavoriteClerkFind(string query, LCatalogOrder order)
    {
        return LCatalogFavorite.LCatalogFavoriteSort(_lFavoriteClerkFavorites.LFavoriteFind(query), order);
    }

    public IReadOnlyList<LCatalogFavorite> LFavoriteClerkFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(
            LFavoriteClerkFind(query, order), favorite => favorite.LCatalogFavoriteEntry.LEntryLanguage);
    }

    public bool LFavoriteClerkCheck(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        return _lFavoriteClerkFavorites.LFavoriteCheck(entryId);
    }

    public void LFavoriteClerkSave(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        _lFavoriteClerkFavorites.LFavoriteSave(entryId);
    }

    public void LFavoriteClerkDelete(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        _lFavoriteClerkFavorites.LFavoriteDelete(entryId);
    }
}
