using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerk
{
    private readonly LIdentity _lDraftClerkIdentity;
    private readonly LLanguageCache _lDraftClerkLanguages;
    private readonly LDraftClerkChip _lDraftClerkChip;
    private readonly LDraftClerkGloss _lDraftClerkGloss;
    private readonly LDraftClerkMedia _lDraftClerkMedia;
    private readonly LDraftClerkMention _lDraftClerkMention;
    private readonly LDraftClerkPanel _lDraftClerkPanel;
    private readonly LDraftClerkReading _lDraftClerkReading;
    private readonly LDraftClerkReflex _lDraftClerkReflex;
    private readonly LDraftClerkSentence _lDraftClerkSentence;

    public LDraftClerk(LRig rig, LIdentity identity, LLanguageCache languages)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(languages);
        _lDraftClerkIdentity = identity;
        _lDraftClerkLanguages = languages;
        _lDraftClerkChip = new LDraftClerkChip(rig.LRigSituations, rig.LRigRegisters, rig.LRigTags, identity);
        _lDraftClerkGloss = new LDraftClerkGloss(identity);
        _lDraftClerkMedia = new LDraftClerkMedia(rig.LRigImages, rig.LRigVideos, identity);
        _lDraftClerkMention = new LDraftClerkMention(rig.LRigEntries, rig.LRigMeanings, identity);
        _lDraftClerkPanel = new LDraftClerkPanel(rig.LRigAuthors, identity);
        _lDraftClerkReading = new LDraftClerkReading(languages, identity);
        _lDraftClerkReflex = new LDraftClerkReflex(languages, identity);
        _lDraftClerkSentence = new LDraftClerkSentence(rig.LRigExamples, identity);
    }

    public LDraft LDraftClerkApply(LDraft draft, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(request);

        if (draft.LDraftSituation is LSituation held
            && _lDraftClerkMedia.LSituationDispatch(held, request) is LSituation dispatched)
        {
            return draft with { LDraftSituation = dispatched };
        }

        return request switch
        {
            LRequestExampleText sent => LDraftClerkPanel.LExampleChange(
                draft,
                example => LDraftClerkMention.LMentionUpdate(example, LStateValue.LStateValueRead(sent.LRequestValue))),
            LRequestMentionAddition sent => _lDraftClerkMention.LMentionAdd(draft, sent),
            LRequestMentionRemoval sent => LDraftClerkMention.LMentionRemove(draft, sent),
            LRequestMentionSense sent => _lDraftClerkMention.LMentionChange(draft, sent),
            LRequestGlossAddition sent => _lDraftClerkGloss.LGlossAdd(draft, sent),
            LRequestGlossRemoval sent => LDraftClerkGloss.LGlossRemove(draft, sent),
            LRequestGlossText sent => LDraftClerkGloss.LGlossChange(draft, sent),
            LRequestGlossLanguage sent => LDraftClerkGloss.LGlossChange(draft, sent),
            LRequestExampleLanguage sent => LDraftClerkPanel.LExampleChange(
                draft, example => example with { LExampleLanguage = sent.LRequestLanguage ?? string.Empty }),
            LRequestExampleReference sent => LDraftClerkPanel.LExampleChange(
                draft,
                example => example with { LExampleSource = LStateAnchor.LStateAnchorRead(sent.LRequestReferenceId) }),
            LRequestReferenceTitle sent => LDraftClerkPanel.LReferenceChange(
                draft,
                reference => reference with { LReferenceTitle = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestReferenceYear sent => LDraftClerkPanel.LReferenceChange(
                draft,
                reference => reference with { LReferenceYear = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestReferenceKind sent => LDraftClerkPanel.LReferenceChange(
                draft, reference => reference with { LReferenceKind = sent.LRequestKind }),
            LRequestReferenceNote sent => LDraftClerkPanel.LReferenceChange(
                draft,
                reference => reference with { LReferenceNote = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestReferenceUrl sent => LDraftClerkPanel.LReferenceChange(
                draft, reference => reference with { LReferenceUrl = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestAuthorState sent => LDraftClerkPanel.LReferenceChange(
                draft,
                reference => reference with
                {
                    LReferenceAuthorState = LStateMark.LStateMarkRead(sent.LRequestState),
                }),
            LRequestReferenceBody sent => LDraftClerkPanel.LReferenceChange(
                draft, reference => LDraftClerkPanel.LReferenceBodyApply(reference, sent)),
            LRequestExampleBody sent => LDraftClerkPanel.LExampleChange(
                draft, example => LDraftClerkPanel.LExampleBodyApply(example, sent)),
            LRequestSituationBody sent => LDraftClerkChip.LSituationChange(
                draft,
                draft.LDraftSituation?.LSituationId ?? sent.LRequestSituationId,
                situation => LDraftClerkPanel.LSituationBodyApply(situation, sent)),
            LRequestAuthorAddition sent => _lDraftClerkPanel.LAuthorAdd(draft, sent),
            LRequestAuthorPick sent => _lDraftClerkPanel.LAuthorInsert(draft, sent),
            LRequestAuthorRemoval sent => LDraftClerkPanel.LAuthorRemove(draft, sent.LRequestAuthorId),
            LRequestAuthorShift sent => LDraftClerkPanel.LAuthorMove(draft, sent),
            LRequestAuthorName sent => LDraftClerkPanel.LAuthorChange(draft, sent.LRequestText),
            LRequestSituationTitle sent => LDraftClerkChip.LSituationChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with { LSituationTitle = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestSituationDescription sent => LDraftClerkChip.LSituationChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with
                {
                    LSituationDescription = LStateValue.LStateValueRead(sent.LRequestValue),
                }),
            LRequestSituationKind sent => LDraftClerkChip.LSituationChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with { LSituationKind = LStateValue.LStateValueRead(sent.LRequestValue) }),
            _ => draft with { LDraftContent = LDraftClerkApply(draft.LDraftContent, request) },
        };
    }

    public LEntryDraft LDraftClerkApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(request);

        return request switch
        {
            LRequestHeadword sent => content with { LEntryDraftHeadword = sent.LRequestText ?? string.Empty },
            LRequestLanguage sent => _lDraftClerkLanguages.LLanguageAnatomyRebuild(
                _lDraftClerkLanguages.LLanguageRespellingRebuild(
                    content with { LEntryDraftLanguage = sent.LRequestText ?? string.Empty })),
            LRequestNote sent => content with { LEntryDraftNote = sent.LRequestText ?? string.Empty },
            LRequestSpeech sent => content with { LEntryDraftSpeeches = sent.LRequestSpeeches ?? [] },
            LRequestCardAddition sent => LCardInsert(content, sent),
            LRequestCardRemoval sent => LDraftClerkCard.LCardRemove(content, sent.LRequestCardId),
            LRequestCardShift sent => LDraftClerkCard.LCardMove(content, sent),
            LRequestCardTitle sent => LDraftClerkCard.LCardChange(
                content,
                sent.LRequestCardId,
                card => card with { LCardDraftTitle = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestCardExpression sent => LDraftClerkCard.LCardChange(
                content,
                sent.LRequestCardId,
                card => card with { LCardDraftExpression = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestCardMeaning sent => LDraftClerkCard.LCardChange(
                content,
                sent.LRequestCardId,
                card => card with { LCardDraftMeaning = LStateValue.LStateValueRead(sent.LRequestValue) }),
            _ => _lDraftClerkReading.LReadingApply(content, request)
                ?? _lDraftClerkReflex.LReflexApply(content, request)
                ?? LDraftListApply(content, request),
        };
    }

    private LEntryDraft LCardInsert(LEntryDraft content, LRequestCardAddition request)
    {
        LCardDraft card = new(
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            0,
            _lDraftClerkIdentity.LIdentityCreate());

        return LDraftClerkCard.LCardInsert(
            content, request.LRequestKind, request.LRequestParentId, request.LRequestPosition, card);
    }

    private LEntryDraft LDraftListApply(LEntryDraft content, LRequest request)
    {
        return request switch
        {
            LRequestTagPick { LRequestCardId: 0 } sent => LCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestRegisterPick { LRequestCardId: 0 } sent => LCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSituationPick { LRequestCardId: 0 } sent => LCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSentenceExample { LRequestCardId: 0 } sent => LCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSentenceReference { LRequestCardId: 0 } sent => LCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSentenceExample { LRequestSentenceId: 0 } sent => LSentenceResolve(
                content, sent.LRequestCardId, id => sent with { LRequestSentenceId = id }),
            LRequestSentenceReference { LRequestSentenceId: 0 } sent => LSentenceResolve(
                content, sent.LRequestCardId, id => sent with { LRequestSentenceId = id }),
            LRequestSentenceAddition sent => _lDraftClerkSentence.LSentenceAdd(content, sent),
            LRequestSentenceRemoval sent => LDraftClerkSentence.LSentenceRemove(content, sent),
            LRequestSentenceShift sent => LDraftClerkSentence.LSentenceMove(content, sent),
            LRequestSentenceExample sent => _lDraftClerkSentence.LSentenceSelect(content, sent),
            LRequestSentenceText sent => _lDraftClerkSentence.LExampleChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                example => LDraftClerkMention.LMentionUpdate(example, LStateValue.LStateValueRead(sent.LRequestValue))),
            LRequestSentenceParticle sent => LDraftClerkSentence.LSentenceChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                sentence => sentence with { LSentenceDraftParticle = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestSentenceDependence sent => LDraftClerkSentence.LSentenceChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                sentence => sentence with
                {
                    LSentenceDraftDependence = LStateValue.LStateValueRead(sent.LRequestValue),
                }),
            LRequestSentenceReference sent => _lDraftClerkSentence.LExampleChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                example => example with
                {
                    LExampleDraftReference = LStateAnchor.LStateAnchorRead(sent.LRequestReferenceId),
                }),
            LRequestSituationAddition sent => _lDraftClerkChip.LSituationAdd(content, sent),
            LRequestSituationPick sent => _lDraftClerkChip.LSituationInsert(content, sent),
            LRequestSituationRemoval sent => LDraftClerkChip.LSituationRemove(content, sent),
            LRequestSituationShift sent => LDraftClerkChip.LSituationMove(content, sent),
            LRequestRegisterAddition sent => _lDraftClerkChip.LRegisterAdd(content, sent),
            LRequestRegisterPick sent => _lDraftClerkChip.LRegisterInsert(content, sent),
            LRequestRegisterRemoval sent => LDraftClerkChip.LRegisterRemove(content, sent),
            LRequestRegisterShift sent => LDraftClerkChip.LRegisterMove(content, sent),
            LRequestRegisterName sent => LDraftClerkChip.LRegisterChange(content, sent),
            LRequestTagAddition sent => _lDraftClerkChip.LTagAdd(content, sent),
            LRequestTagPick sent => _lDraftClerkChip.LTagInsert(content, sent),
            LRequestTagRemoval sent => LDraftClerkChip.LTagRemove(content, sent),
            LRequestTagShift sent => LDraftClerkChip.LTagMove(content, sent),
            LRequestTagText sent => LDraftClerkChip.LTagChange(content, sent),
            LRequestTranslationPick sent => LDraftClerkChip.LTranslationInsert(content, sent),
            LRequestTranslationRemoval sent => LDraftClerkChip.LTranslationRemove(content, sent),
            LRequestTranslationShift sent => LDraftClerkChip.LTranslationMove(content, sent),
            LRequestImageAddition sent => _lDraftClerkMedia.LImageApply(content, sent.LRequestCardId, sent),
            LRequestImagePick sent => _lDraftClerkMedia.LImageApply(content, sent.LRequestCardId, sent),
            LRequestImageRemoval sent => _lDraftClerkMedia.LImageApply(content, sent.LRequestCardId, sent),
            LRequestImageShift sent => _lDraftClerkMedia.LImageApply(content, sent.LRequestCardId, sent),
            LRequestImageLocation sent => LDraftClerkMedia.LImageChange(content, sent),
            LRequestVideoAddition sent => _lDraftClerkMedia.LVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoPick sent => _lDraftClerkMedia.LVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoRemoval sent => _lDraftClerkMedia.LVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoShift sent => _lDraftClerkMedia.LVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoLocation sent => LDraftClerkMedia.LVideoChange(
                content,
                sent.LRequestVideoId,
                video => video with { LVideoDraftLocation = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestVideoSpan sent => LDraftClerkMedia.LVideoChange(
                content,
                sent.LRequestVideoId,
                video => video with { LVideoDraftSpan = LStateValue.LStateValueRead(sent.LRequestValue) }),
            _ => throw new ArgumentException("The request kind is not one the clerk applies.", nameof(request)),
        };
    }

    private LEntryDraft LCardResolve(LEntryDraft content, Func<long, LRequest> retarget)
    {
        if (content.LEntryDraftMeanings.Count == 0)
        {
            content = LCardInsert(content, new LRequestCardAddition(0, LCardKind.LCardKindMeaning, 0, 0));
        }

        return LDraftListApply(content, retarget(content.LEntryDraftMeanings[0].LCardDraftId));
    }

    private LEntryDraft LSentenceResolve(LEntryDraft content, long cardId, Func<long, LRequest> retarget)
    {
        LCardDraft card = LDraftClerkCard.LCardFind(content, cardId) ?? throw new LRefusal(LRefusal.LRefusalCard);
        if (card.LCardDraftSentence.Count == 0)
        {
            content = _lDraftClerkSentence.LSentenceAdd(content, new LRequestSentenceAddition(0, cardId, 0));
            card = LDraftClerkCard.LCardFind(content, cardId) ?? throw new LRefusal(LRefusal.LRefusalCard);
        }

        return LDraftListApply(content, retarget(card.LCardDraftSentence[0].LSentenceDraftId));
    }
}
