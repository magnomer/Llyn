using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LFavorite> LEngineFavoriteFind(string query)
    {
        return new LFavoriteArchive(_lEngineDatabase).LFavoriteFind(query);
    }

    public bool LEngineFavoriteCheck(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        return new LFavoriteArchive(_lEngineDatabase).LFavoriteCheck(entryId);
    }

    public void LEngineFavoriteSave(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        new LFavoriteArchive(_lEngineDatabase).LFavoriteSave(entryId);
    }

    public void LEngineFavoriteDelete(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        new LFavoriteArchive(_lEngineDatabase).LFavoriteDelete(entryId);
    }
}
