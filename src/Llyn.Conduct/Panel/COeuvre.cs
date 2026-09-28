using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

internal static class COeuvre
{
    internal static IReadOnlyList<CCatalogReference> COeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .Select(static row => new CCatalogReference(
                row.LCatalogReferenceStored.LReferenceId,
                row.LCatalogReferenceName,
                row.LCatalogReferenceByline,
                COeuvreCreditRead(
                    row.LCatalogReferenceWriter,
                    row.LCatalogReferenceStored.LReferenceAuthorState.LStateMarkUncertain),
                CFolio.CFolioStateRead(row.LCatalogReferenceStored.LReferenceYear),
                row.LCatalogReferenceUsage,
                row.LCatalogReferenceChosen))
            .ToList();
    }

    private static CStateValue COeuvreCreditRead(string? credit, bool uncertain)
    {
        return credit is null ? new CStateValue(string.Empty, uncertain, false) : new CStateValue(credit, false, true);
    }
}
