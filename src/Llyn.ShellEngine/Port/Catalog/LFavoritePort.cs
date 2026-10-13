using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LFavoritePort
{
    IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista);

    bool LEngineFavoriteCheck(long entryId);

    void LEngineFavoriteSave(long entryId);

    void LEngineFavoriteDelete(long entryId);
}
