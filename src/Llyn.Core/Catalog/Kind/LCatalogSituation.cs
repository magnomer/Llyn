using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogSituation(
    LSituation LCatalogSituationStored,
    int LCatalogSituationUsage,
    bool LCatalogSituationChosen = false)
{
    public string LCatalogSituationName { get; init; } = LCatalogSituationStored.LSituationTitle.LStateValueShow();

    public string LCatalogSituationKind { get; init; } = LCatalogSituationStored.LSituationKind.LStateValueShow();

    public string LCatalogSituationCount => LCatalog.LCatalogUsageFormat(LCatalogSituationUsage);

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
                    StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(
                    row => row.LCatalogSituationStored.LSituationTitle.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(row => row.LCatalogSituationStored.LSituationId)],
            LCatalogOrder.LCatalogOrderUsage => LCatalog.LCatalogUsageSort(
                rows,
                static row => row.LCatalogSituationUsage,
                static row => row.LCatalogSituationStored.LSituationTitle.LStateValueShow(),
                static row => row.LCatalogSituationStored.LSituationId),
            _ => [.. rows
                .OrderBy(
                    row => row.LCatalogSituationStored.LSituationTitle.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(row => row.LCatalogSituationStored.LSituationId)],
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
