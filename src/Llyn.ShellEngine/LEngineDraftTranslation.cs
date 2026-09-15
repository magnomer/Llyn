using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineCourtApply(
        IReadOnlyDictionary<long, LDraft> loaded,
        IReadOnlyDictionary<long, LOutcome> settled,
        IReadOnlyList<LCourt> deferred)
    {
        foreach (LCourt link in deferred)
        {
            if (!loaded.TryGetValue(link.LCourtOwnerId, out LDraft? owner)
                || !settled.TryGetValue(link.LCourtOwnerId, out LOutcome? made)
                || !settled.TryGetValue(link.LCourtTargetId, out LOutcome? target))
            {
                continue;
            }

            long entryId = target.LOutcomeEntry.LEntryId;
            LEngineCourtApply(
                owner.LDraftContent.LEntryDraftMeanings, made, link.LCourtTargetId, entryId, false);
            LEngineCourtApply(
                owner.LDraftContent.LEntryDraftCollocations, made, link.LCourtTargetId, entryId, true);
        }
    }

    private void LEngineCourtApply(
        IReadOnlyList<LCardDraft> cards, LOutcome made, long draftId, long entryId, bool collocation)
    {
        foreach (LCardDraft card in cards)
        {
            if (LEngineTranslationCheck(card.LCardDraftTranslation, draftId))
            {
                long ownerId = made.LOutcomeIdentity.TryGetValue(card.LCardDraftId, out long real)
                    ? real
                    : card.LCardDraftId;
                if (ownerId > 0)
                {
                    LEngineTranslationAppend(ownerId, entryId, collocation);
                }
            }

            if (!collocation)
            {
                LEngineCourtApply(card.LCardDraftChild, made, draftId, entryId, false);
            }
        }
    }

    private static bool LEngineTranslationCheck(IReadOnlyList<long> translations, long id)
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

    private void LEngineTranslationAppend(long ownerId, long entryId, bool collocation)
    {
        LTranslationArchive translations = new(_lEngineDatabase);
        List<long> ids = [];
        foreach (LTranslation held in collocation
            ? translations.LTranslationCollocationRead(ownerId)
            : translations.LTranslationMeaningRead(ownerId))
        {
            ids.Add(held.LTranslationEntryId);
        }

        if (ids.Contains(entryId))
        {
            return;
        }

        ids.Add(entryId);
        LEngineTranslationSave(ownerId, ids, collocation);
    }

    private LEntryDraft LEngineTranslationSettle(
        LEntryDraft content, IReadOnlyDictionary<long, long> identity)
    {
        return content with
        {
            LEntryDraftMeanings = LEngineTranslationSettle(content.LEntryDraftMeanings, identity),
            LEntryDraftCollocations = LEngineTranslationSettle(content.LEntryDraftCollocations, identity),
        };
    }

    private IReadOnlyList<LCardDraft> LEngineTranslationSettle(
        IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<long, long> identity)
    {
        LEntryArchive entries = new(_lEngineDatabase);
        List<LCardDraft> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<long> translations = new(card.LCardDraftTranslation.Count);
            foreach (long held in card.LCardDraftTranslation)
            {
                long translation = identity.TryGetValue(held, out long settled) ? settled : held;
                if (translation > 0 && entries.LEntryRead(translation) is not null)
                {
                    translations.Add(translation);
                }
            }

            written.Add(card with
            {
                LCardDraftTranslation = translations,
                LCardDraftChild = LEngineTranslationSettle(card.LCardDraftChild, identity),
            });
        }

        return written;
    }
}
