using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkSentence
{
    private readonly LExampleVault _lDraftClerkExamples;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkSentence(LExampleVault examples, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkExamples = examples;
        _lDraftClerkIdentity = identity;
    }

    public LEntryDraft LSentenceAdd(LEntryDraft content, LRequestSentenceAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LSentenceDraft sentence = new(
            null,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            _lDraftClerkIdentity.LIdentityCreate());

        return LSentenceApply(
            content,
            request.LRequestCardId,
            sentences => LDraftClerkList.LDraftListAdd(sentences, sentence, request.LRequestPosition));
    }

    public static LEntryDraft LSentenceRemove(LEntryDraft content, LRequestSentenceRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LSentenceApply(
            content,
            request.LRequestCardId,
            sentences => LDraftClerkList.LDraftListRemove(
                sentences, request.LRequestSentenceId, static row => row.LSentenceDraftId));
    }

    public static LEntryDraft LSentenceMove(LEntryDraft content, LRequestSentenceShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LSentenceApply(
            content,
            request.LRequestCardId,
            sentences => LDraftClerkList.LDraftListMove(
                sentences, request.LRequestSentenceId, request.LRequestPosition, static row => row.LSentenceDraftId));
    }

    public LEntryDraft LSentenceSelect(LEntryDraft content, LRequestSentenceExample request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestExampleId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalExample);
        }

        LExample stored = _lDraftClerkExamples.LExampleRead(request.LRequestExampleId)
            ?? throw new LRefusal(LRefusal.LRefusalExample);

        LExampleDraft example = LExampleDraft.LExampleDraftCreate(stored);

        return LSentenceChange(
            content,
            request.LRequestCardId,
            request.LRequestSentenceId,
            sentence => sentence with { LSentenceDraftExample = example });
    }

    public LEntryDraft LExampleChange(
        LEntryDraft content, long cardId, long sentenceId, Func<LExampleDraft, LExampleDraft> change)
    {
        ArgumentNullException.ThrowIfNull(change);

        return LSentenceChange(content, cardId, sentenceId, sentence =>
        {
            LExampleDraft? example = change(sentence.LSentenceDraftExample ?? new LExampleDraft(
                LStateValue.LStateValueUnspecified,
                0,
                LStateAnchor.LStateAnchorUnspecified));

            if (example.LExampleDraftId == 0)
            {
                bool blank = example.LExampleDraftText.LStateValueEmpty
                    && example.LExampleDraftReference.LStateAnchorEmpty
                    && example.LExampleDraftGloss.Count == 0;
                example = blank
                    ? null
                    : example with { LExampleDraftId = _lDraftClerkIdentity.LIdentityCreate() };
            }

            return sentence with { LSentenceDraftExample = example };
        });
    }

    public static LEntryDraft LSentenceChange(
        LEntryDraft content, long cardId, long sentenceId, Func<LSentenceDraft, LSentenceDraft> change)
    {
        return LSentenceApply(
            content,
            cardId,
            sentences => LDraftClerkList.LDraftListChange(
                sentences, sentenceId, static row => row.LSentenceDraftId, change)
                ?? throw new LRefusal(LRefusal.LRefusalItem));
    }

    private static LEntryDraft LSentenceApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LSentenceDraft>, IReadOnlyList<LSentenceDraft>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftSentence = change(card.LCardDraftSentence) });
    }
}
