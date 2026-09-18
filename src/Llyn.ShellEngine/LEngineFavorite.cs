using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query)
    {
        lock (_lEngineGate)
        {
            return new LFavoriteArchive(_lEngineDatabase).LFavoriteFind(query);
        }
    }

    public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return LCatalogFavorite.LCatalogFavoriteSort(
                new LFavoriteArchive(_lEngineDatabase).LFavoriteFind(query),
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
            return new LFavoriteArchive(_lEngineDatabase).LFavoriteCheck(entryId);
        }
    }

    public void LEngineFavoriteSave(long entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            new LFavoriteArchive(_lEngineDatabase).LFavoriteSave(entryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }

    public void LEngineFavoriteDelete(long entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            new LFavoriteArchive(_lEngineDatabase).LFavoriteDelete(entryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }
}
