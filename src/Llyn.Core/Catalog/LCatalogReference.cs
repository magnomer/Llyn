using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCatalogReference(
    LReference LCatalogReferenceStored,
    string LCatalogReferenceName,
    IReadOnlyList<LAuthor> LCatalogReferenceCredit,
    int LCatalogReferenceUsage)
{
    public static LCatalogReference LCatalogReferenceCreate(
        LReference reference,
        IReadOnlyList<LAuthor>? credits,
        int usage)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return new LCatalogReference(reference, reference.LReferenceNameRead(), credits ?? [], usage);
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
                    ? row.LCatalogReferenceStored.LReferenceAuthorState
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

        if (query.Length == 0)
        {
            return true;
        }

        LReference reference = LCatalogReferenceStored;
        if (LCatalog.LCatalogTextMatch(reference.LReferenceTitle.LStateValueShow(), query)
            || LCatalog.LCatalogTextMatch(reference.LReferenceProgram.LStateValueShow(), query)
            || LCatalog.LCatalogTextMatch(reference.LReferenceChannel.LStateValueShow(), query)
            || LCatalog.LCatalogTextMatch(reference.LReferenceUrl.LStateValueShow(), query))
        {
            return true;
        }

        foreach (LAuthor author in LCatalogReferenceCredit)
        {
            if (LCatalog.LCatalogTextMatch(author.LAuthorName, query))
            {
                return true;
            }
        }

        return false;
    }
}
