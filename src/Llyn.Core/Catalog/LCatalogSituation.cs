using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogSituation(
    LSituation LCatalogSituationStored,
    int LCatalogSituationUsage,
    bool LCatalogSituationChosen = false)
{
    public static LCatalogSituation LCatalogSituationCreate(LSituation situation, int usage)
    {
        ArgumentNullException.ThrowIfNull(situation);

        return new LCatalogSituation(situation, usage);
    }

    public static IReadOnlyList<LCatalogSituation> LCatalogSituationSort(
        IReadOnlyList<LCatalogSituation> rows,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return order switch
        {
            LCatalogOrder.LCatalogOrderKind => [.. rows
                .OrderBy(
                    row => row.LCatalogSituationStored.LSituationKind.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderUsage => [.. rows
                .OrderByDescending(row => row.LCatalogSituationUsage)],
            _ => [.. rows
                .OrderBy(
                    row => row.LCatalogSituationStored.LSituationTitle.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)],
        };
    }

    public bool LCatalogSituationMatch(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query.Length == 0
            || LCatalog.LCatalogTextMatch(LCatalogSituationStored.LSituationTitle.LStateValueShow(), query)
            || LCatalog.LCatalogTextMatch(LCatalogSituationStored.LSituationDescription.LStateValueShow(), query)
            || LCatalog.LCatalogTextMatch(LCatalogSituationStored.LSituationKind.LStateValueShow(), query);
    }
}
