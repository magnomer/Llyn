using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogReference(
    LReference LCatalogReferenceStored,
    string LCatalogReferenceName,
    string LCatalogReferenceByline,
    IReadOnlyList<LAuthor> LCatalogReferenceCredit,
    int LCatalogReferenceUsage,
    bool LCatalogReferenceChosen = false)
{
    private static readonly char[] LCatalogReferenceBreak = [' ', '	', '(', ')', ','];

    public static LCatalogReference LCatalogReferenceCreate(
        LReference reference,
        IReadOnlyList<LAuthor>? credits,
        int usage)
    {
        ArgumentNullException.ThrowIfNull(reference);

        IReadOnlyList<LAuthor> credited = credits ?? [];
        return new LCatalogReference(
            reference,
            reference.LReferenceNameRead(),
            reference.LReferenceBylineRead(credited),
            credited,
            usage);
    }

    public static IReadOnlyList<LCatalogReference> LCatalogReferenceSort(
        IReadOnlyList<LCatalogReference> rows,
        LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return order switch
        {
            LCatalogOrder.LCatalogOrderYear => [.. rows
                .OrderBy(row => row.LCatalogReferenceStored.LReferenceYear.LStateValueState)
                .ThenBy(
                    row => row.LCatalogReferenceStored.LReferenceYear.LStateValueShow(),
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderAuthor => [.. rows
                .OrderBy(row => row.LCatalogReferenceCredit.Count == 0
                    ? row.LCatalogReferenceStored.LReferenceAuthorState.LStateMarkState
                    : LState.LStateSpecified)
                .ThenBy(
                    row => row.LCatalogReferenceCredit.Count == 0
                        ? string.Empty
                        : row.LCatalogReferenceCredit[0].LAuthorName,
                    StringComparer.CurrentCultureIgnoreCase)],
            LCatalogOrder.LCatalogOrderUsage => [.. rows
                .OrderByDescending(row => row.LCatalogReferenceUsage)],
            _ => [.. rows
                .OrderBy(row => row.LCatalogReferenceName, StringComparer.CurrentCultureIgnoreCase)],
        };
    }

    public bool LCatalogReferenceMatch(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        foreach (string term in query.Split(LCatalogReferenceBreak, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!LCatalogReferenceCheck(term))
            {
                return false;
            }
        }

        return true;
    }

    private bool LCatalogReferenceCheck(string term)
    {
        LReference reference = LCatalogReferenceStored;
        if (LCatalog.LCatalogTextMatch(reference.LReferenceTitle.LStateValueShow(), term)
            || LCatalog.LCatalogTextMatch(reference.LReferenceYear.LStateValueShow(), term)
            || LCatalog.LCatalogTextMatch(reference.LReferenceNote.LStateValueShow(), term)
            || LCatalog.LCatalogTextMatch(reference.LReferenceUrl.LStateValueShow(), term))
        {
            return true;
        }

        foreach (LAuthor author in LCatalogReferenceCredit)
        {
            if (LCatalog.LCatalogTextMatch(author.LAuthorName, term))
            {
                return true;
            }
        }

        return false;
    }
}
