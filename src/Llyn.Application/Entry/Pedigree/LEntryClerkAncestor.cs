using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LEntryClerkAncestor
{
    public static void LAncestorUpdate(
        LAncestorVault ancestors, long entryId, LEntryDraft draft, List<LRevisionDelta> changes)
    {
        ArgumentNullException.ThrowIfNull(ancestors);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(changes);

        IReadOnlyList<LAncestorDraft> stored = LAncestorDraft.LAncestorDraftCreate(ancestors.LAncestorRead(entryId));
        IReadOnlyList<LAncestorDraft> current = draft.LEntryDraftAncestors;
        if (LAncestorMatch(stored, current))
        {
            return;
        }

        IReadOnlyList<LAncestorDraft> trimmed = current
            .Select(static box => box with
            {
                LAncestorDraftText = box.LAncestorDraftText.Trim(),
                LAncestorDraftGloss = box.LAncestorDraftGloss.Trim(),
            })
            .ToList();
        ancestors.LAncestorSave(entryId, LAncestorDraft.LAncestorDraftResolve(trimmed));

        changes.Add(new LRevisionDelta(
            entryId,
            "ancestor",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            current.Count == 0 ? null : LAncestorFormat(trimmed)));
    }

    public static bool LAncestorMatch(IReadOnlyList<LAncestorDraft> one, IReadOnlyList<LAncestorDraft> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        if (one.Count != other.Count)
        {
            return false;
        }

        IReadOnlyList<(int LAncestorOrderChild, LAncestorDraft LAncestorOrderBox)> held = LAncestorOrderRead(one);
        IReadOnlyList<(int LAncestorOrderChild, LAncestorDraft LAncestorOrderBox)> named = LAncestorOrderRead(other);
        if (held.Count != one.Count || named.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < held.Count; index++)
        {
            LAncestorDraft first = held[index].LAncestorOrderBox;
            LAncestorDraft second = named[index].LAncestorOrderBox;
            if (held[index].LAncestorOrderChild != named[index].LAncestorOrderChild
                || first.LAncestorDraftTarget != second.LAncestorDraftTarget
                || first.LAncestorDraftTongue != second.LAncestorDraftTongue
                || first.LAncestorDraftRelation != second.LAncestorDraftRelation
                || !string.Equals(
                    first.LAncestorDraftText.Trim(), second.LAncestorDraftText.Trim(), StringComparison.Ordinal)
                || !string.Equals(
                    first.LAncestorDraftLanguage, second.LAncestorDraftLanguage, StringComparison.Ordinal)
                || !string.Equals(
                    first.LAncestorDraftGloss.Trim(), second.LAncestorDraftGloss.Trim(), StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<(int LAncestorOrderChild, LAncestorDraft LAncestorOrderBox)> LAncestorOrderRead(
        IReadOnlyList<LAncestorDraft> boxes)
    {
        List<(int LAncestorOrderChild, LAncestorDraft LAncestorOrderBox)> ordered = new(boxes.Count);
        Queue<(long? LAncestorOrderId, int LAncestorOrderIndex)> pending = new([(null, -1)]);
        while (pending.Count > 0 && ordered.Count <= boxes.Count)
        {
            (long? id, int index) = pending.Dequeue();
            foreach (LAncestorDraft box in boxes)
            {
                if (box.LAncestorDraftChild == id)
                {
                    ordered.Add((index, box));
                    pending.Enqueue((box.LAncestorDraftId, ordered.Count - 1));
                }
            }
        }

        return ordered;
    }

    private static string LAncestorFormat(IReadOnlyList<LAncestorDraft> boxes)
    {
        return string.Join(
            ", ",
            LAncestorOrderRead(boxes)
                .Select(static held => held.LAncestorOrderBox.LAncestorDraftText)
                .Where(static text => text.Length > 0));
    }
}
