using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CAncestor(
    long CAncestorId,
    string CAncestorKey,
    int CAncestorDepth,
    string CAncestorText,
    string CAncestorLanguage,
    long? CAncestorEntry,
    bool CAncestorBorrowed,
    string CAncestorGloss,
    long? CAncestorRelation,
    bool CAncestorRestorable)
{
    public bool CAncestorLinked => CAncestorEntry is not null;

    internal static IReadOnlyList<CAncestor> CAncestorRead(IReadOnlyList<LLineage> lineage, long? restorable)
    {
        ArgumentNullException.ThrowIfNull(lineage);

        List<CAncestor> rows = [];
        Stack<(LLineage, int, string)> pending = new();
        for (int index = lineage.Count - 1; index >= 0; index--)
        {
            pending.Push((lineage[index], 0, string.Empty));
        }

        while (pending.Count > 0)
        {
            (LLineage box, int depth, string below) = pending.Pop();
            bool linked = box.LLineageEntryId is not null;
            string key = below.Length == 0
                ? box.LLineageId.ToString(CultureInfo.InvariantCulture)
                : below + "/" + box.LLineageId.ToString(CultureInfo.InvariantCulture);
            rows.Add(new CAncestor(
                box.LLineageId,
                key,
                depth,
                box.LLineageText,
                box.LLineageLanguage,
                box.LLineageEntryId,
                box.LLineageBorrowed,
                box.LLineageGloss,
                box.LLineageRelationId,
                linked && !box.LLineageBorrowed && box.LLineageId == restorable));
            for (int index = box.LLineageSources.Count - 1; index >= 0; index--)
            {
                pending.Push((box.LLineageSources[index], depth + 1, key));
            }
        }

        return rows;
    }
}
