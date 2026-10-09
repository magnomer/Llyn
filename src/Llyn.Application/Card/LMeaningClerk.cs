using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMeaningClerk
{
    private readonly LMeaningVault _lMeaningClerkMeanings;
    private readonly LCardClerk _lMeaningClerkCards;

    public LMeaningClerk(LRig rig, LCardClerk cards)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(cards);
        _lMeaningClerkMeanings = rig.LRigLexicon.LRigLexiconMeanings;
        _lMeaningClerkCards = cards;
    }

    public IReadOnlyList<LMeaning> LMeaningClerkScan(long entryId)
    {
        return _lMeaningClerkMeanings.LMeaningRead(entryId);
    }

    public static IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LMeaningClerkSort(
        IReadOnlyList<LMeaning> meanings, string unknown)
    {
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(unknown);

        Func<long, List<LMeaning>> childrenRead = parent =>
        {
            List<LMeaning> children = meanings
                .Where(meaning => (meaning.LMeaningParentId ?? 0) == parent && meaning.LMeaningId != parent)
                .ToList();
            children.Sort((left, right) => left.LMeaningPosition.CompareTo(right.LMeaningPosition));
            return children;
        };

        List<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows = [];
        Stack<(int, int, List<LMeaning>)> path = [];
        path.Push((0, 0, childrenRead(0)));
        while (path.Count > 0)
        {
            (int depth, int next, List<LMeaning> children) = path.Pop();
            if (next >= children.Count)
            {
                continue;
            }

            LMeaning meaning = children[next];
            path.Push((depth, next + 1, children));
            rows.Add((meaning.LMeaningId, meaning.LMeaningName.Length > 0 ? meaning.LMeaningName : unknown, depth));
            path.Push((depth + 1, 0, childrenRead(meaning.LMeaningId)));
        }

        return rows;
    }

    public void LMeaningClerkSave(
        long entryId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        List<LRevisionDelta> changes,
        Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(changes);
        ArgumentNullException.ThrowIfNull(identity);

        LMeaningVault meanings = _lMeaningClerkMeanings;

        Dictionary<long, LMeaning> stored = [];
        List<long> storedOrder = [];
        foreach (LMeaning row in meanings.LMeaningRead(entryId))
        {
            stored[row.LMeaningId] = row;
            storedOrder.Add(row.LMeaningId);
        }

        HashSet<long> named = [];
        LMeaningClerkScan(cards, stored, named);

        HashSet<long> gone = [];
        List<long> deleted = [];
        foreach (long id in storedOrder)
        {
            bool dropped = !named.Contains(id);
            bool shadowed = false;
            HashSet<long> walked = [id];
            long? up = stored[id].LMeaningParentId;
            while (up is long ancestor && stored.TryGetValue(ancestor, out LMeaning? above)
                && walked.Add(ancestor))
            {
                shadowed |= !named.Contains(ancestor);
                up = above.LMeaningParentId;
            }

            if (dropped || shadowed)
            {
                gone.Add(id);
            }

            if (dropped && !shadowed)
            {
                deleted.Add(id);
            }
        }

        foreach (long id in deleted)
        {
            changes.Add(new LRevisionDelta(
                id, "sense", "delete", stored[id].LMeaningDefinition.LStateValueShow()));
            meanings.LMeaningDelete(id);
        }

        LMeaningClerkApply(
            entryId,
            null,
            cards,
            language,
            changes,
            stored,
            gone,
            new HashSet<long>(),
            identity);
    }

    private void LMeaningClerkApply(
        long entryId,
        long? parentId,
        IReadOnlyList<LCardDraft> cards,
        string language,
        List<LRevisionDelta> changes,
        IReadOnlyDictionary<long, LMeaning> stored,
        ISet<long> gone,
        ISet<long> applied,
        Dictionary<long, long> identity)
    {
        LMeaningVault meanings = _lMeaningClerkMeanings;
        List<long> order = [];
        foreach (LCardDraft card in LCardClerkField.LCardRead(cards))
        {
            bool reuse = stored.ContainsKey(card.LCardDraftId)
                && !gone.Contains(card.LCardDraftId)
                && applied.Add(card.LCardDraftId);

            long rowId;
            if (reuse)
            {
                LMeaning row = stored[card.LCardDraftId];
                if (row.LMeaningParentId != parentId)
                {
                    meanings.LMeaningParentUpdate(row.LMeaningId, parentId);
                }

                if (row.LMeaningTitle != card.LCardDraftTitle
                    || row.LMeaningDefinition != card.LCardDraftMeaning
                    || row.LMeaningPosition != order.Count
                    || card.LCardDraftTitle.LStateValueUnreadable
                    || card.LCardDraftMeaning.LStateValueUnreadable)
                {
                    meanings.LMeaningUpdate(row with
                    {
                        LMeaningTitle = card.LCardDraftTitle,
                        LMeaningDefinition = card.LCardDraftMeaning,
                    });
                    changes.Add(new LRevisionDelta(
                        row.LMeaningId, "sense", "update", card.LCardDraftMeaning.LStateValueShow()));
                }

                rowId = row.LMeaningId;
            }
            else
            {
                LMeaning created = meanings.LMeaningCreate(new LMeaning(
                    0,
                    entryId,
                    parentId,
                    0,
                    card.LCardDraftTitle,
                    card.LCardDraftMeaning));
                changes.Add(new LRevisionDelta(
                    created.LMeaningId,
                    "sense",
                    "create",
                    card.LCardDraftMeaning.LStateValueShow()));
                rowId = created.LMeaningId;
                LIdentity.LIdentityRecord(identity, card.LCardDraftId, rowId);
            }

            order.Add(rowId);
            _lMeaningClerkCards.LCardClerkSync(rowId, card, language, false, identity);
            LMeaningClerkApply(
                entryId,
                rowId,
                card.LCardDraftChild,
                language,
                changes,
                stored,
                gone,
                applied,
                identity);
        }

        meanings.LMeaningOrderSet(entryId, parentId, order);
    }

    private static void LMeaningClerkScan(
        IReadOnlyList<LCardDraft> cards,
        IReadOnlyDictionary<long, LMeaning> stored,
        ISet<long> named)
    {
        foreach (LCardDraft card in LCardClerkField.LCardRead(cards))
        {
            if (stored.ContainsKey(card.LCardDraftId))
            {
                named.Add(card.LCardDraftId);
            }

            LMeaningClerkScan(card.LCardDraftChild, stored, named);
        }
    }
}
