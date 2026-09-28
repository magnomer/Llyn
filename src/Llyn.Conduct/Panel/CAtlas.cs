using System;
using Llyn.Core;

namespace Llyn.Conduct;

internal static class CAtlas
{
    internal static CCatalogSituation CAtlasRowRead(LCatalogSituation row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CCatalogSituation(
            row.LCatalogSituationStored.LSituationId,
            row.LCatalogSituationName,
            row.LCatalogSituationUsage,
            CFolio.CFolioStateRead(row.LCatalogSituationStored.LSituationKind),
            row.LCatalogSituationChosen);
    }
}
