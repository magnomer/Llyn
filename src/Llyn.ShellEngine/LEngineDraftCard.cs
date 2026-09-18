using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal IReadOnlyList<LCardDraft> LEngineDraftMove(long id, bool collocation, int from, int target)
    {
        lock (_lEngineGate)
        {
            LEngineDraftValidate(id);
            LDraft draft = LEngineDraftLoad(id);
            List<LCardDraft> cards = new(
                collocation
                    ? draft.LDraftContent.LEntryDraftCollocations
                    : draft.LDraftContent.LEntryDraftMeanings);

            if (cards.Count != 0)
            {
                int origin = Math.Clamp(from, 0, cards.Count - 1);
                int landing = Math.Clamp(target, 0, cards.Count - 1);

                LCardDraft moved = cards[origin];
                cards.RemoveAt(origin);
                cards.Insert(landing, moved);
            }

            return LEngineCardApply(draft, collocation, cards);
        }
    }

    internal IReadOnlyList<LCardDraft> LEngineDraftNormalize(long id, bool collocation)
    {
        lock (_lEngineGate)
        {
            LEngineDraftValidate(id);
            LDraft draft = LEngineDraftLoad(id);
            List<LCardDraft> cards = new(
                collocation
                    ? draft.LDraftContent.LEntryDraftCollocations
                    : draft.LDraftContent.LEntryDraftMeanings);

            return LEngineCardApply(draft, collocation, cards);
        }
    }

    internal long LEngineCardCreate()
    {
        lock (_lEngineGate)
        {
            return LEngineIdentityCreate();
        }
    }

    private IReadOnlyList<LCardDraft> LEngineCardApply(
        LDraft draft, bool collocation, List<LCardDraft> cards)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            cards[index] = card with
            {
                LCardDraftPosition = index + 1,
                LCardDraftId = card.LCardDraftId == 0
                    ? LEngineIdentityCreate()
                    : card.LCardDraftId,
            };
        }

        LEntryDraft content = collocation
            ? draft.LDraftContent with { LEntryDraftCollocations = cards }
            : draft.LDraftContent with { LEntryDraftMeanings = cards };

        LDraft saved = draft with { LDraftContent = content };
        LEngineChronicleRecord(draft, saved, null);
        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, saved);
        return cards;
    }
}
