using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LDraft LEngineGlossAdd(LDraft draft, LRequestGlossAddition request)
    {
        LGlossDraft added = new(
            LEngineIdentityCreate(),
            request.LRequestLanguage ?? string.Empty,
            LStateValue.LStateValueUnspecified);

        return LEngineGlossApply(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            example => LEngineListAdd(example.LExampleDraftGloss, added, request.LRequestPosition));
    }

    private static LDraft LEngineGlossRemove(LDraft draft, LRequestGlossRemoval request)
    {
        return LEngineGlossApply(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            example => LEngineListRemove(
                example.LExampleDraftGloss, request.LRequestGlossId, static row => row.LGlossDraftId));
    }

    private static LDraft LEngineGlossChange(LDraft draft, LRequestGlossText request)
    {
        return LEngineGlossChange(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            request.LRequestGlossId,
            gloss => gloss with { LGlossDraftText = LEngineValueRead(request.LRequestValue) });
    }

    private static LDraft LEngineGlossChange(LDraft draft, LRequestGlossLanguage request)
    {
        return LEngineGlossChange(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            request.LRequestGlossId,
            gloss => gloss with { LGlossDraftLanguage = request.LRequestLanguage ?? string.Empty });
    }

    private static LDraft LEngineGlossChange(
        LDraft draft, long cardId, long sentenceId, long glossId, Func<LGlossDraft, LGlossDraft> change)
    {
        return LEngineGlossApply(draft, cardId, sentenceId, example =>
            LEngineListChange(example.LExampleDraftGloss, glossId, static row => row.LGlossDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem));
    }

    private static LDraft LEngineGlossApply(
        LDraft draft,
        long cardId,
        long sentenceId,
        Func<LExampleDraft, IReadOnlyList<LGlossDraft>> change)
    {
        if (cardId == 0 && sentenceId == 0)
        {
            LExample held = draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample);
            LExampleDraft written = LExampleDraft.LExampleDraftCreate(held);
            return draft with { LDraftExample = held with { LExampleGloss = LEngineGlossRead(change(written)) } };
        }

        LEntryDraft content = LEngineSentenceChange(draft.LDraftContent, cardId, sentenceId, sentence =>
        {
            LExampleDraft example = sentence.LSentenceDraftExample ?? throw new LRefusal(LRefusal.LRefusalItem);
            return sentence with { LSentenceDraftExample = example with { LExampleDraftGloss = change(example) } };
        });

        return draft with { LDraftContent = content };
    }

    private static void LEngineGlossRecord(
        Dictionary<long, long> identity, IReadOnlyList<LGlossDraft> drafts, IReadOnlyList<LGloss> stored)
    {
        for (int index = 0; index < drafts.Count && index < stored.Count; index++)
        {
            if (drafts[index].LGlossDraftId < 0)
            {
                LEngineIdentityRecord(identity, drafts[index].LGlossDraftId, stored[index].LGlossId);
            }
        }
    }

    private static IReadOnlyList<LGloss> LEngineGlossRead(IReadOnlyList<LGlossDraft> drafts)
    {
        List<LGloss> glosses = new(drafts.Count);
        foreach (LGlossDraft draft in drafts)
        {
            glosses.Add(draft.LGlossDraftResolve());
        }

        return glosses;
    }
}
