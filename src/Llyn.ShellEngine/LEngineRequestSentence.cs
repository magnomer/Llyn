using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineSentenceAdd(LEntryDraft content, LRequestSentenceAddition request)
    {
        LSentenceDraft sentence = new(
            null,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LEngineIdentityCreate());

        return LEngineSentenceApply(
            content,
            request.LRequestCardId,
            sentences => LEngineListAdd(sentences, sentence, request.LRequestPosition));
    }

    private static LEntryDraft LEngineSentenceRemove(LEntryDraft content, LRequestSentenceRemoval request)
    {
        return LEngineSentenceApply(
            content,
            request.LRequestCardId,
            sentences => LEngineListRemove(sentences, request.LRequestSentenceId, static row => row.LSentenceDraftId));
    }

    private static LEntryDraft LEngineSentenceMove(LEntryDraft content, LRequestSentenceShift request)
    {
        return LEngineSentenceApply(
            content,
            request.LRequestCardId,
            sentences => LEngineListMove(
                sentences, request.LRequestSentenceId, request.LRequestPosition, static row => row.LSentenceDraftId));
    }

    private LEntryDraft LEngineSentenceSelect(LEntryDraft content, LRequestSentenceExample request)
    {
        if (request.LRequestExampleId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalExample);
        }

        LExample stored = new LExampleArchive(_lEngineDatabase).LExampleRead(request.LRequestExampleId)
            ?? throw new LRefusal(LRefusal.LRefusalExample);

        LExampleDraft example = new(
            stored.LExampleText,
            stored.LExampleId,
            stored.LExampleSource,
            stored.LExampleTranslation,
            stored.LExampleLanguage);

        return LEngineSentenceChange(
            content,
            request.LRequestCardId,
            request.LRequestSentenceId,
            sentence => sentence with { LSentenceDraftExample = example });
    }

    private LEntryDraft LEngineExampleChange(
        LEntryDraft content, long cardId, long sentenceId, Func<LExampleDraft, LExampleDraft> change)
    {
        return LEngineSentenceChange(content, cardId, sentenceId, sentence =>
        {
            LExampleDraft? example = change(sentence.LSentenceDraftExample ?? new LExampleDraft(
                LStateValue.LStateValueUnspecified,
                0,
                LStateAnchor.LStateAnchorUnspecified,
                LStateValue.LStateValueUnspecified));

            if (example.LExampleDraftId == 0)
            {
                bool blank = example.LExampleDraftText.LStateValueEmpty
                    && example.LExampleDraftReference.LStateAnchorEmpty;
                example = blank ? null : example with { LExampleDraftId = LEngineIdentityCreate() };
            }

            return sentence with { LSentenceDraftExample = example };
        });
    }

    private static LEntryDraft LEngineSentenceChange(
        LEntryDraft content, long cardId, long sentenceId, Func<LSentenceDraft, LSentenceDraft> change)
    {
        return LEngineSentenceApply(
            content,
            cardId,
            sentences => LEngineListChange(sentences, sentenceId, static row => row.LSentenceDraftId, change)
                ?? throw new LRefusal(LRefusal.LRefusalItem));
    }

    private static LEntryDraft LEngineSentenceApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LSentenceDraft>, IReadOnlyList<LSentenceDraft>> change)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftSentence = change(card.LCardDraftSentence) });
    }
}
