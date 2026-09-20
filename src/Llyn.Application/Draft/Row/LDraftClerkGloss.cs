using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkGloss
{
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkGloss(LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkIdentity = identity;
    }

    public LDraft LGlossAdd(LDraft draft, LRequestGlossAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LGlossDraft added = new(
            _lDraftClerkIdentity.LIdentityCreate(),
            request.LRequestLanguage ?? string.Empty,
            LStateValue.LStateValueUnspecified);

        return LGlossApply(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            example => LDraftClerkList.LDraftListAdd(example.LExampleDraftGloss, added, request.LRequestPosition));
    }

    public static LDraft LGlossRemove(LDraft draft, LRequestGlossRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LGlossApply(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            example => LDraftClerkList.LDraftListRemove(
                example.LExampleDraftGloss, request.LRequestGlossId, static row => row.LGlossDraftId));
    }

    public static LDraft LGlossChange(LDraft draft, LRequestGlossText request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LGlossChange(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            request.LRequestGlossId,
            gloss => gloss with { LGlossDraftText = LStateValue.LStateValueRead(request.LRequestValue) });
    }

    public static LDraft LGlossChange(LDraft draft, LRequestGlossLanguage request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LGlossChange(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            request.LRequestGlossId,
            gloss => gloss with { LGlossDraftLanguage = request.LRequestLanguage ?? string.Empty });
    }

    private static LDraft LGlossChange(
        LDraft draft, long cardId, long sentenceId, long glossId, Func<LGlossDraft, LGlossDraft> change)
    {
        return LGlossApply(draft, cardId, sentenceId, example =>
            LDraftClerkList.LDraftListChange(
                example.LExampleDraftGloss, glossId, static row => row.LGlossDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem));
    }

    private static LDraft LGlossApply(
        LDraft draft,
        long cardId,
        long sentenceId,
        Func<LExampleDraft, IReadOnlyList<LGlossDraft>> change)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (cardId == 0 && sentenceId == 0)
        {
            LExample held = draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample);
            LExampleDraft written = LExampleDraft.LExampleDraftCreate(held);
            return draft with { LDraftExample = held with { LExampleGloss = LGlossRead(change(written)) } };
        }

        LEntryDraft content = LDraftClerkSentence.LSentenceChange(draft.LDraftContent, cardId, sentenceId, sentence =>
        {
            LExampleDraft example = sentence.LSentenceDraftExample ?? throw new LRefusal(LRefusal.LRefusalItem);
            return sentence with { LSentenceDraftExample = example with { LExampleDraftGloss = change(example) } };
        });

        return draft with { LDraftContent = content };
    }

    public static IReadOnlyList<LGloss> LGlossRead(IReadOnlyList<LGlossDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LGloss> glosses = new(drafts.Count);
        foreach (LGlossDraft draft in drafts)
        {
            glosses.Add(draft.LGlossDraftResolve());
        }

        return glosses;
    }
}
