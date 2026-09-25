using System.Collections.Generic;

namespace Llyn.Core;

public interface LFavoriteVault
{
    void LFavoriteSave(long entryId);

    void LFavoriteDelete(long entryId);

    bool LFavoriteCheck(long entryId);

    IReadOnlyList<LCatalogFavorite> LFavoriteFind(string query);
}
