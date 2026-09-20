using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogExample(
    LExample LCatalogExampleStored,
    string LCatalogExampleSource,
    int LCatalogExampleUsage,
    bool LCatalogExampleChosen = false)
{
    public string LCatalogExampleName { get; init; } = LCatalogExampleStored.LExampleText.LStateValueShow();

    public static LCatalogExample LCatalogExampleCreate(LExample example, string? source, int usage)
    {
        ArgumentNullException.ThrowIfNull(example);

        return new LCatalogExample(example, source ?? string.Empty, usage);
    }

    public static IReadOnlyList<LCatalogExample> LCatalogExampleSort(
        IReadOnlyList<LCatalogExample> rows,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return order switch
        {
            LCatalogOrder.LCatalogOrderLanguage => [.. rows
                .OrderBy(
                    row => row.LCatalogExampleStored.LExampleLanguage,
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderSource => [.. rows
                .OrderBy(row => row.LCatalogExampleSource, StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderUsage => [.. rows
                .OrderByDescending(row => row.LCatalogExampleUsage)],
            _ => [.. rows
                .OrderBy(
                    row => row.LCatalogExampleStored.LExampleText.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)],
        };
    }

    public bool LCatalogExampleMatch(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query.Length == 0
            || LCatalog.LCatalogTextMatch(LCatalogExampleStored.LExampleText.LStateValueShow(), query)
            || LCatalogExampleStored.LExampleGloss.Any(
                gloss => LCatalog.LCatalogTextMatch(gloss.LGlossText.LStateValueShow(), query))
            || LCatalog.LCatalogTextMatch(LCatalogExampleSource, query);
    }
}
