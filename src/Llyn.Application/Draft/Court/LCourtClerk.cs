using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCourtClerk
{
    private readonly LDraftVault _lCourtClerkDrafts;
    private readonly LEntryVault _lCourtClerkEntries;
    private readonly LIdentity _lCourtClerkIdentity;
    private readonly LChronicleClerk _lCourtClerkChronicle;
    private readonly LTranslationClerk _lCourtClerkTranslations;

    public LCourtClerk(LRig rig, LIdentity identity, LChronicleClerk chronicle, LTranslationClerk translations)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(chronicle);
        ArgumentNullException.ThrowIfNull(translations);
        _lCourtClerkDrafts = rig.LRigDraft;
        _lCourtClerkEntries = rig.LRigEntries;
        _lCourtClerkIdentity = identity;
        _lCourtClerkChronicle = chronicle;
        _lCourtClerkTranslations = translations;
    }

    public LCourt LCourtClerkSave(long ownerId, long targetId, string headword, string language)
    {
        ArgumentOutOfRangeException.ThrowIfZero(ownerId);
        ArgumentOutOfRangeException.ThrowIfZero(targetId);
        ArgumentException.ThrowIfNullOrWhiteSpace(headword);

        LCourt link = new(
            _lCourtClerkIdentity.LIdentityCreate(),
            ownerId,
            targetId,
            headword,
            language ?? string.Empty);

        _lCourtClerkDrafts.LDraftCourtSave(link);
        return link;
    }

    public IReadOnlyList<LCourt> LCourtClerkScan(long ownerId)
    {
        List<LCourt> owned = [];
        foreach (LCourt link in _lCourtClerkDrafts.LDraftCourtScan())
        {
            if (link.LCourtOwnerId == ownerId)
            {
                owned.Add(link);
            }
        }

        return owned;
    }

    public LCourt? LCourtClerkFind(long ownerId, long targetId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(ownerId);
        ArgumentOutOfRangeException.ThrowIfZero(targetId);

        foreach (LCourt link in LCourtClerkScan(ownerId))
        {
            if (link.LCourtTargetId == targetId)
            {
                return link;
            }
        }

        return null;
    }

    public void LCourtClerkDelete(long linkId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(linkId);
        _lCourtClerkDrafts.LDraftCourtDelete(linkId);
    }

    public void LCourtClerkSettle(long draftId, long realId)
    {
        foreach (long ownerId in _lCourtClerkDrafts.LDraftCourtSettle(draftId, realId))
        {
            _lCourtClerkChronicle.LChronicleClerkClear(ownerId);
        }
    }

    public void LCourtClerkApply(
        IReadOnlyDictionary<long, LDraft> loaded,
        IReadOnlyDictionary<long, LOutcome> settled,
        IReadOnlyList<LCourt> deferred)
    {
        ArgumentNullException.ThrowIfNull(loaded);
        ArgumentNullException.ThrowIfNull(settled);
        ArgumentNullException.ThrowIfNull(deferred);

        foreach (LCourt link in deferred)
        {
            if (!loaded.TryGetValue(link.LCourtOwnerId, out LDraft? owner)
                || !settled.TryGetValue(link.LCourtOwnerId, out LOutcome? made)
                || !settled.TryGetValue(link.LCourtTargetId, out LOutcome? target))
            {
                continue;
            }

            long entryId = target.LOutcomeEntry.LEntryId;
            LCourtClerkApply(owner.LDraftContent.LEntryDraftMeanings, made, link.LCourtTargetId, entryId, false);
            LCourtClerkApply(owner.LDraftContent.LEntryDraftCollocations, made, link.LCourtTargetId, entryId, true);
        }
    }

    public LEntryDraft LTranslationSettle(LEntryDraft content, IReadOnlyDictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(identity);

        return content with
        {
            LEntryDraftMeanings = LTranslationSettle(content.LEntryDraftMeanings, identity),
            LEntryDraftCollocations = LTranslationSettle(content.LEntryDraftCollocations, identity),
        };
    }

    private IReadOnlyList<LCardDraft> LTranslationSettle(
        IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<long, long> identity)
    {
        List<LCardDraft> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<long> translations = new(card.LCardDraftTranslation.Count);
            foreach (long held in card.LCardDraftTranslation)
            {
                long translation = identity.TryGetValue(held, out long settled) ? settled : held;
                if (translation > 0 && _lCourtClerkEntries.LEntryRead(translation) is not null)
                {
                    translations.Add(translation);
                }
            }

            written.Add(card with
            {
                LCardDraftTranslation = translations,
                LCardDraftChild = LTranslationSettle(card.LCardDraftChild, identity),
            });
        }

        return written;
    }

    private void LCourtClerkApply(
        IReadOnlyList<LCardDraft> cards, LOutcome made, long draftId, long entryId, bool collocation)
    {
        foreach (LCardDraft card in cards)
        {
            if (LTranslationCheck(card.LCardDraftTranslation, draftId))
            {
                long ownerId = made.LOutcomeIdentity.TryGetValue(card.LCardDraftId, out long real)
                    ? real
                    : card.LCardDraftId;
                if (ownerId > 0)
                {
                    LTranslationAppend(ownerId, entryId, collocation);
                }
            }

            if (!collocation)
            {
                LCourtClerkApply(card.LCardDraftChild, made, draftId, entryId, false);
            }
        }
    }

    private static bool LTranslationCheck(IReadOnlyList<long> translations, long id)
    {
        foreach (long held in translations)
        {
            if (held == id)
            {
                return true;
            }
        }

        return false;
    }

    private void LTranslationAppend(long ownerId, long entryId, bool collocation)
    {
        List<long> ids = [];
        foreach (LTranslation held in _lCourtClerkTranslations.LTranslationClerkRead(ownerId, collocation))
        {
            ids.Add(held.LTranslationEntryId);
        }

        if (ids.Contains(entryId))
        {
            return;
        }

        ids.Add(entryId);
        _lCourtClerkTranslations.LTranslationClerkSave(ownerId, ids, collocation);
    }
}
