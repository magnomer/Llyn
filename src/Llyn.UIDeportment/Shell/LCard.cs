using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LCard
{
    private readonly LDesk _lCardDesk;

    private readonly LDraftPort _lDraftPort;

    private readonly LEntryPort _lEntryPort;

    private readonly LPhonologyPort _lPhonologyPort;

    internal LCard(LDesk desk, LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);

        _lCardDesk = desk;
        _lDraftPort = drafts;
        _lEntryPort = entries;
        _lPhonologyPort = phonology;
    }

    public IReadOnlyList<CVistaRow> LCardProspectFind(string word)
    {
        return LSplice.LSpliceBuild(
            _lDraftPort.LEngineProspectFind(word),
            static row => new CVistaRow(
                row.LVistaRowId,
                row.LVistaRowHeadword,
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty,
                row.LVistaRowName));
    }

    public long? LCardTranslationResolve(string word, long? entryId)
    {
        return _lDraftPort.LEngineTranslationResolve(word, entryId)?.LEntryId;
    }

    public long LCardCourtStart(long ownerId, string origin, string headword, string language)
    {
        return _lDraftPort.LEngineCourtStart(ownerId, origin, headword, language).LCourtTargetId;
    }

    public void LCardCourtDelete(long ownerId, long targetId)
    {
        LCardCourtDelete(_lDraftPort.LEngineCourtFind(ownerId, targetId), targetId);
    }

    private void LCardCourtDelete(LCourt? link, long targetId)
    {
        if (link is null)
        {
            return;
        }

        _lDraftPort.LEngineCourtDelete(link.LCourtId);
        _lDraftPort.LEngineDraftDelete(targetId);
    }

    public IReadOnlyList<CRegister> LCardRegisterFind(string word, string language)
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineRegisterFind(word, language),
            static row => new CRegister(row.LRegisterId, row.LRegisterName.LStateValueShow()));
    }

    public IReadOnlyList<CCatalogSituation> LCardSituationFind(string word)
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineSituationFind(word, LCatalogOrder.LCatalogOrderUsage),
            static row => new CCatalogSituation(
                row.LCatalogSituationStored.LSituationId,
                row.LCatalogSituationStored.LSituationTitle.LStateValueShow(),
                row.LCatalogSituationUsage));
    }

    public IReadOnlyList<CCatalogReference> LCardReferenceFind(string word)
    {
        return LOeuvre.LOeuvreReferenceRead(_lEntryPort.LEngineReferenceFind(word, LCatalogOrder.LCatalogOrderUsage));
    }

    public IReadOnlyList<CCatalogReference> LCardReferenceFind()
    {
        return LOeuvre.LOeuvreReferenceRead(
            _lEntryPort.LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor));
    }

    public void LCardCitationSet(long cardId, long sentenceId, string title)
    {
        _lCardDesk.LDeskSend(new LRequestSentenceReference(
            _lCardDesk.LDeskId,
            cardId,
            sentenceId,
            _lEntryPort.LEngineCitationResolve(_lCardDesk.LDeskId, cardId, sentenceId, title)));
    }

    public IReadOnlyList<CTag> LCardTagFind(string word)
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineTagFind(word, LCatalogOrder.LCatalogOrderUsage),
            static row => new CTag(row.LTagId, row.LTagText));
    }

    public CSentenceOrder LCardOrderRead(string language)
    {
        return LCardOrderRead(_lPhonologyPort.LEngineOrderRead(language));
    }

    public IReadOnlyList<string> LCardParticleRead(string language)
    {
        return _lPhonologyPort.LEngineParticleRead(language);
    }

    public IReadOnlyList<string> LCardDependenceRead(string language)
    {
        return _lPhonologyPort.LEngineDependenceRead(language);
    }

    public void LCardEtymologySet(string text)
    {
        _lCardDesk.LDeskDefer(new LRequestEtymologyText(_lCardDesk.LDeskId, text));
    }

    public void LCardEtymonAdd(long entryId)
    {
        _lCardDesk.LDeskSend(new LRequestEtymonAddition(_lCardDesk.LDeskId, entryId, int.MaxValue));
    }

    public void LCardEtymonRemove(long entryId)
    {
        _lCardDesk.LDeskSend(new LRequestEtymonRemoval(_lCardDesk.LDeskId, entryId));
    }

    public void LCardMentionSave(string text, int start, int length, long entryId)
    {
        LCardMentionSend(_lDraftPort.LEngineSpanRead(text, start, length), entryId);
    }

    public void LCardMentionDelete(string text, int start, int length)
    {
        LCardMentionSend(LCardEtymologyRead().LEtymologyDraftFind(_lDraftPort.LEngineSpanRead(text, start, length)), 0);
    }

    public bool LCardMentionCheck(string text, int start, int length)
    {
        return LCardEtymologyRead().LEtymologyDraftFind(_lDraftPort.LEngineSpanRead(text, start, length)) is not null;
    }

    private LEtymologyDraft LCardEtymologyRead()
    {
        return _lCardDesk.LDeskTenure?.LTenureRead()?.LDraftContent.LEntryDraftEtymology ?? new LEtymologyDraft();
    }

    private void LCardMentionSend(LMentionDraft? span, long entryId)
    {
        if (span is null)
        {
            return;
        }

        _lCardDesk.LDeskSend(new LRequestEtymologyMention(
            _lCardDesk.LDeskId, span.LMentionDraftOffset, span.LMentionDraftLength, entryId));
    }

    internal static CSentenceOrder LCardOrderRead(LSentenceOrder order)
    {
        return new CSentenceOrder(order.LSentenceOrderParticle, order.LSentenceOrderDependence);
    }

    internal static IReadOnlyList<CTranslationTarget> LCardTargetRead(IReadOnlyList<LTranslationTarget> targets)
    {
        return LSplice.LSpliceBuild(
            targets,
            static target => new CTranslationTarget(
                target.LTranslationTargetId,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage));
    }

    internal static CStateValue LCardStateRead(LStateValue value)
    {
        return new CStateValue(value.LStateValueShow(), value.LStateValueUncertain, value.LStateValueLegible);
    }

    internal static CEntryDraft LCardEntryRead(LEntryDraft draft)
    {
        return new CEntryDraft(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftLanguage,
            draft.LEntryDraftNote,
            draft.LEntryDraftAudio,
            LSounding.LSoundingPrimaryRead(draft.LEntryDraftPronunciation),
            LSounding.LSoundingPronunciationRead(draft.LEntryDraftAccents),
            LSplice.LSpliceBuild(
                draft.LEntryDraftSpeeches,
                static speech => new CSpeechDraft(speech.LSpeechDraftValue, speech.LSpeechDraftName)),
            LCardSheetRead(draft.LEntryDraftMeanings),
            LCardSheetRead(draft.LEntryDraftCollocations),
            LSounding.LSoundingTranscriptionRead(draft.LEntryDraftTranscriptions),
            LSounding.LSoundingReflexRead(draft.LEntryDraftReflexes),
            new CEtymologyDraft(
                draft.LEntryDraftEtymology.LEtymologyDraftText,
                LCardMentionRead(draft.LEntryDraftEtymology.LEtymologyDraftMentions)));
    }

    internal static IReadOnlyList<CCardDraft> LCardSheetRead(IReadOnlyList<LCardDraft> cards)
    {
        return LSplice.LSpliceBuild(
            cards,
            static card => new CCardDraft(
                card.LCardDraftId,
                card.LCardDraftPosition,
                LCardStateRead(card.LCardDraftTitle),
                LCardStateRead(card.LCardDraftExpression),
                LCardStateRead(card.LCardDraftMeaning),
                LSplice.LSpliceBuild(card.LCardDraftSentence, LCardSentenceRead),
                LSplice.LSpliceBuild(
                    card.LCardDraftSituation,
                    static row => new CSituationDraft(row.LSituationDraftId, LCardStateRead(row.LSituationDraftTitle))),
                LSplice.LSpliceBuild(
                    card.LCardDraftRegister,
                    static row => new CRegisterDraft(row.LRegisterDraftId, LCardStateRead(row.LRegisterDraftName))),
                card.LCardDraftTranslation,
                LSplice.LSpliceBuild(
                    card.LCardDraftTag, static row => new CTagDraft(row.LTagDraftId, row.LTagDraftText)),
                LCardImageRead(card.LCardDraftImage),
                LCardVideoRead(card.LCardDraftVideo),
                LCardSheetRead(card.LCardDraftChild)));
    }

    private static CSentenceDraft LCardSentenceRead(LSentenceDraft sentence)
    {
        return new CSentenceDraft(
            sentence.LSentenceDraftId,
            LCardExampleRead(sentence.LSentenceDraftExample),
            LCardStateRead(sentence.LSentenceDraftParticle),
            LCardStateRead(sentence.LSentenceDraftDependence));
    }

    private static CExampleDraft? LCardExampleRead(LExampleDraft? example)
    {
        return example is null
            ? null
            : new CExampleDraft(
                LCardStateRead(example.LExampleDraftText),
                example.LExampleDraftReference.LStateAnchorShown,
                LCardGlossRead(example.LExampleDraftGloss),
                LCardMentionRead(example.LExampleDraftMention));
    }

    internal static IReadOnlyList<CGlossDraft> LCardGlossRead(IReadOnlyList<LGlossDraft> glosses)
    {
        return LSplice.LSpliceBuild(
            glosses,
            static gloss => new CGlossDraft(
                gloss.LGlossDraftId, gloss.LGlossDraftLanguage, LCardStateRead(gloss.LGlossDraftText)));
    }

    internal static IReadOnlyList<CMentionDraft> LCardMentionRead(IReadOnlyList<LMentionDraft> mentions)
    {
        return LSplice.LSpliceBuild(mentions, LCardMentionRead);
    }

    internal static IReadOnlyList<CImageDraft> LCardImageRead(IReadOnlyList<LImageDraft> images)
    {
        return LSplice.LSpliceBuild(
            images,
            static image => new CImageDraft(image.LImageDraftId, LCardStateRead(image.LImageDraftLocation)));
    }

    internal static IReadOnlyList<CVideoDraft> LCardVideoRead(IReadOnlyList<LVideoDraft> videos)
    {
        return LSplice.LSpliceBuild(
            videos,
            static video => new CVideoDraft(
                video.LVideoDraftId,
                LCardStateRead(video.LVideoDraftLocation),
                LCardStateRead(video.LVideoDraftSpan)));
    }

    internal static CMentionDraft LCardMentionRead(LMentionDraft mention)
    {
        return new CMentionDraft(
            mention.LMentionDraftId,
            mention.LMentionDraftEntry,
            mention.LMentionDraftOffset,
            mention.LMentionDraftLength,
            mention.LMentionDraftSense);
    }

}
