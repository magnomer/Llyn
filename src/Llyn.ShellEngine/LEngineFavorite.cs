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

    public bool LEngineFavoriteCheck(string entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
            return new LFavoriteArchive(_lEngineDatabase).LFavoriteCheck(entryId);
        }
    }

    public void LEngineFavoriteSave(string entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
            new LFavoriteArchive(_lEngineDatabase).LFavoriteSave(entryId);
        }
    }

    public void LEngineFavoriteDelete(string entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
            new LFavoriteArchive(_lEngineDatabase).LFavoriteDelete(entryId);
        }
    }
}
