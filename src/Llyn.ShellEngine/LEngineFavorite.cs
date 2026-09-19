using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query)
    {
        lock (_lEngineGate)
        {
            return _lEngineFavorites.LFavoriteFind(query);
        }
    }

    public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return LCatalogFavorite.LCatalogFavoriteSort(
                _lEngineFavorites.LFavoriteFind(query),
                order);
        }
    }

    public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(
            LEngineFavoriteFind(query, order), favorite => favorite.LFavoriteEntry.LEntryLanguage);
    }

    public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        lock (_lEngineGate)
        {
            IReadOnlyList<LFavorite> favorites = LEngineFavoriteFind(
                vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            List<LEntry> entries = new(favorites.Count);
            foreach (LFavorite favorite in favorites)
            {
                entries.Add(favorite.LFavoriteEntry);
            }

            return LEngineVistaBuild(entries, vista.LVistaChosen);
        }
    }

    public bool LEngineFavoriteCheck(long entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            return _lEngineFavorites.LFavoriteCheck(entryId);
        }
    }

    public void LEngineFavoriteSave(long entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            _lEngineFavorites.LFavoriteSave(entryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }

    public void LEngineFavoriteDelete(long entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            _lEngineFavorites.LFavoriteDelete(entryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }
}
