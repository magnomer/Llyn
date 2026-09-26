using System;
using System.Collections.Generic;
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
        _lMeaningClerkMeanings = rig.LRigMeanings;
        _lMeaningClerkCards = cards;
    }

    public IReadOnlyList<LMeaning> LMeaningClerkScan(long entryId)
    {
        return _lMeaningClerkMeanings.LMeaningRead(entryId);
    }

    public void LMeaningClerkCreate(
        long entryId, long? parentId, LCardDraft card, string language, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(identity);

        LMeaning meaning = _lMeaningClerkMeanings.LMeaningCreate(new LMeaning(
            0,
            entryId,
            parentId,
            0,
            card.LCardDraftTitle,
            card.LCardDraftMeaning));

        LIdentity.LIdentityRecord(identity, card.LCardDraftId, meaning.LMeaningId);
        _lMeaningClerkCards.LCardClerkSync(meaning.LMeaningId, card, language, false, identity);

        foreach (LCardDraft child in LCardClerkField.LCardRead(card.LCardDraftChild))
        {
            LMeaningClerkCreate(entryId, meaning.LMeaningId, child, language, identity);
        }
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
        foreach (long dropped in storedOrder)
        {
            LMeaning row = stored[dropped];
            if (row.LMeaningParentId is long parent && gone.Contains(parent))
            {
                gone.Add(dropped);
                continue;
            }

            if (named.Contains(dropped))
            {
                continue;
            }

            gone.Add(dropped);
            changes.Add(new LRevisionDelta(
                dropped, "sense", "delete", row.LMeaningDefinition.LStateValueShow()));
            meanings.LMeaningDelete(dropped);
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
