using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerk
{
    private readonly LIdentity _lDraftClerkIdentity;
    private readonly LDraftClerkEntry _lDraftClerkEntry;
    private readonly LDraftClerkChip _lDraftClerkChip;
    private readonly LSituationChip _lDraftClerkSituation;
    private readonly LRegisterChip _lDraftClerkRegister;
    private readonly LDraftClerkEtymology _lDraftClerkEtymology;
    private readonly LDraftClerkGloss _lDraftClerkGloss;
    private readonly LDraftClerkMedia _lDraftClerkMedia;
    private readonly LDraftClerkMention _lDraftClerkMention;
    private readonly LDraftClerkPanel _lDraftClerkPanel;
    private readonly LDraftClerkReading _lDraftClerkReading;
    private readonly LDraftClerkReflex _lDraftClerkReflex;
    private readonly LDraftClerkSentence _lDraftClerkSentence;
    private readonly LDraftClerkMint _lDraftClerkMint;

    public LDraftClerk(LRig rig, LIdentity identity, LLanguageCache languages)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(languages);
        _lDraftClerkIdentity = identity;
        _lDraftClerkEntry = new LDraftClerkEntry(languages);
        _lDraftClerkChip = new LDraftClerkChip(rig.LRigContext.LRigContextTags, identity);
        _lDraftClerkSituation = new LSituationChip(rig.LRigContext.LRigContextSituations, identity);
        _lDraftClerkRegister = new LRegisterChip(rig.LRigContext.LRigContextRegisters, identity);
        _lDraftClerkEtymology = new LDraftClerkEtymology(rig.LRigEntries, identity);
        _lDraftClerkGloss = new LDraftClerkGloss(identity);
        _lDraftClerkMedia = new LDraftClerkMedia(
            rig.LRigCitation.LRigCitationImages,
            rig.LRigCitation.LRigCitationVideos,
            identity);
        _lDraftClerkMention = new LDraftClerkMention(rig.LRigEntries, rig.LRigLexicon.LRigLexiconMeanings, identity);
        _lDraftClerkPanel = new LDraftClerkPanel(rig.LRigCitation.LRigCitationAuthors, identity);
        _lDraftClerkReading = new LDraftClerkReading(languages, identity);
        _lDraftClerkReflex = new LDraftClerkReflex(languages, identity);
        _lDraftClerkSentence = new LDraftClerkSentence(rig.LRigSentence.LRigSentenceExamples, identity);
        _lDraftClerkMint = new LDraftClerkMint(identity);
    }

    public LEntryDraft LDraftClerkNormalize(LEntryDraft content)
    {
        return _lDraftClerkMint.LDraftNormalize(content);
    }

    public LDraft LDraftClerkApply(LDraft draft, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(request);

        return _lDraftClerkMedia.LSituationDispatch(draft, request)
            ?? _lDraftClerkPanel.LPanelApply(draft, request)
            ?? _lDraftClerkMention.LMentionApply(draft, request)
            ?? _lDraftClerkGloss.LGlossApply(draft, request)
            ?? LSituationChip.LSituationChipApply(draft, request)
            ?? draft with { LDraftContent = LDraftClerkApply(draft.LDraftContent, request) };
    }

    public LEntryDraft LDraftClerkApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(request);

        return _lDraftClerkEntry.LEntryApply(content, request)
            ?? LDraftClerkCard.LCardApply(content, request, _lDraftClerkIdentity)
            ?? _lDraftClerkReading.LReadingApply(content, request)
            ?? _lDraftClerkReflex.LReflexApply(content, request)
            ?? _lDraftClerkEtymology.LEtymologyApply(content, request)
            ?? LDraftListApply(content, request);
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
            _ => _lDraftClerkSentence.LSentenceApply(content, request)
                ?? _lDraftClerkSituation.LSituationChipApply(content, request)
                ?? _lDraftClerkRegister.LRegisterChipApply(content, request)
                ?? _lDraftClerkChip.LTagApply(content, request)
                ?? LDraftClerkChip.LTranslationApply(content, request)
                ?? _lDraftClerkMedia.LImageApply(content, request)
                ?? _lDraftClerkMedia.LVideoApply(content, request)
                ?? throw new ArgumentException("The request kind is not one the clerk applies.", nameof(request)),
        };
    }

    private LEntryDraft LCardResolve(LEntryDraft content, Func<long, LRequest> retarget)
    {
        if (content.LEntryDraftMeanings.Count == 0)
        {
            content = LDraftClerkCard.LCardInsert(
                content, new LRequestCardAddition(0, LCardKind.LCardKindMeaning, 0, 0), _lDraftClerkIdentity);
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
