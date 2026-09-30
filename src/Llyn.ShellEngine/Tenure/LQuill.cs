using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuill
{
    private readonly LTenure _lQuillTenure;

    public LQuill(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillTenure = tenure;
    }

    public void LQuillAuthorSet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        _lQuillTenure.LTenureRequestDefer(new LRequestAuthorName(_lQuillTenure.LTenureId, name));
    }

    public void LQuillExampleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestExampleText(_lQuillTenure.LTenureId, new LStateWritten(text, false)));
    }

    public void LQuillSpeakerSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        _lQuillTenure.LTenureRequestApply(new LRequestExampleLanguage(_lQuillTenure.LTenureId, language));
    }

    public void LQuillReferenceSet(long reference)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestExampleReference(_lQuillTenure.LTenureId, reference));
    }

    public void LQuillAnchorSet(long reflex, long fanqie, bool anchored)
    {
        _lQuillTenure.LTenureRequestApply(
            new LRequestReflexAnchor(_lQuillTenure.LTenureId, reflex, fanqie, anchored));
    }

    public void LQuillGlossRemove(long card, long sentence, long gloss)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestGlossRemoval(_lQuillTenure.LTenureId, card, sentence, gloss));
    }

    public void LQuillGlossSet(long card, long sentence, long gloss, string? language, string? text)
    {
        if (language is not null)
        {
            _lQuillTenure.LTenureRequestApply(
                new LRequestGlossLanguage(_lQuillTenure.LTenureId, card, sentence, gloss, language));
            return;
        }

        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestGlossText(_lQuillTenure.LTenureId, card, sentence, gloss, new LStateWritten(text)));
    }

    public void LQuillMentionRemove(long card, long sentence, long mention)
    {
        _lQuillTenure.LTenureRequestApply(
            new LRequestMentionRemoval(_lQuillTenure.LTenureId, card, sentence, mention));
    }

    public void LQuillTranscriptionSet(long transcription, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestTranscriptionText(_lQuillTenure.LTenureId, transcription, text));
    }

    public void LQuillSchemeSet(long transcription, string scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        _lQuillTenure.LTenureRequestApply(
            new LRequestTranscriptionScheme(_lQuillTenure.LTenureId, transcription, scheme));
    }

    public void LQuillTranscriptionAdd(long transcription)
    {
        if (_lQuillTenure.LTenureTranscriptionRead()?.LTranscriptionSheetScheme is not string scheme)
        {
            return;
        }

        IReadOnlyList<LTranscriptionDraft> spelled =
            _lQuillTenure.LTenureRead()?.LDraftContent.LEntryDraftTranscriptions ?? [];
        _lQuillTenure.LTenureRequestApply(new LRequestTranscriptionAddition(
            _lQuillTenure.LTenureId, scheme, LDraftClerkReading.LTranscriptionPositionRead(spelled, transcription)));
    }

    public void LQuillTranscriptionRemove(long transcription)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestTranscriptionRemoval(_lQuillTenure.LTenureId, transcription));
    }

    public void LQuillReadingSet(LForay foray, string phonetic, string variety)
    {
        ArgumentNullException.ThrowIfNull(foray);
        ArgumentNullException.ThrowIfNull(phonetic);
        ArgumentNullException.ThrowIfNull(variety);

        LEntryDraft? content = _lQuillTenure.LTenureRead()?.LDraftContent;
        long target = foray.LForayTarget;
        if (foray.LForaySchemed)
        {
            if (LQuillRowCheck(content?.LEntryDraftTranscriptions, target, static row => row.LTranscriptionDraftId))
            {
                _lQuillTenure.LTenureRequestApply(
                    new LRequestTranscriptionText(_lQuillTenure.LTenureId, target, phonetic));
            }

            return;
        }

        if (foray.LForayPrimary)
        {
            _lQuillTenure.LTenureRequestApply(new LRequestIpa(_lQuillTenure.LTenureId, phonetic));
        }
        else if (LQuillRowCheck(content?.LEntryDraftPronunciations, target, static row => row.LPronunciationDraftId))
        {
            _lQuillTenure.LTenureRequestApply(
                new LRequestPronunciationIpa(_lQuillTenure.LTenureId, target, phonetic));
        }
        else
        {
            return;
        }

        _lQuillTenure.LTenureVarietySet(foray.LForayPrimary, target, variety);
    }

    private static bool LQuillRowCheck<LQuillRow>(IReadOnlyList<LQuillRow>? rows, long id, Func<LQuillRow, long> key)
    {
        return rows is not null && id != 0 && LDraftClerkList.LDraftListFind(rows, id, key) >= 0;
    }

    public void LQuillEtymologySet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(new LRequestEtymologyText(_lQuillTenure.LTenureId, text));
    }

    public void LQuillEtymonAdd(long entry, int position)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestEtymonAddition(_lQuillTenure.LTenureId, entry, position));
    }

    public void LQuillEtymonRemove(long entry)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestEtymonRemoval(_lQuillTenure.LTenureId, entry));
    }

    public void LQuillMentionSave(int offset, int length, long entry)
    {
        _lQuillTenure.LTenureRequestApply(
            new LRequestEtymologyMention(_lQuillTenure.LTenureId, offset, length, entry));
    }

    public void LQuillCitationSet(long card, long sentence, long reference)
    {
        _lQuillTenure.LTenureRequestApply(
            new LRequestSentenceReference(_lQuillTenure.LTenureId, card, sentence, reference));
    }

    public void LQuillSentenceAdd(long card, int position)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestSentenceAddition(_lQuillTenure.LTenureId, card, position));
    }

    public void LQuillSentenceRemove(long card, long sentence)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestSentenceRemoval(_lQuillTenure.LTenureId, card, sentence));
    }

    public void LQuillSentenceSet(long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestSentenceText(_lQuillTenure.LTenureId, card, sentence, new LStateWritten(text)));
    }

    public void LQuillParticleSet(long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestSentenceParticle(_lQuillTenure.LTenureId, card, sentence, new LStateWritten(text)));
    }

    public void LQuillDependenceSet(long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestSentenceDependence(_lQuillTenure.LTenureId, card, sentence, new LStateWritten(text)));
    }

    public void LQuillTitleSet(long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestCardTitle(_lQuillTenure.LTenureId, card, new LStateWritten(text)));
    }

    public void LQuillExpressionSet(long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestCardExpression(_lQuillTenure.LTenureId, card, new LStateWritten(text)));
    }

    public void LQuillMeaningSet(long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestCardMeaning(_lQuillTenure.LTenureId, card, new LStateWritten(text)));
    }

    public void LQuillTitleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(new LRequestReferenceTitle(_lQuillTenure.LTenureId, new LStateWritten(text)));
    }

    public void LQuillYearSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(new LRequestReferenceYear(_lQuillTenure.LTenureId, new LStateWritten(text)));
    }

    public void LQuillUrlSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(new LRequestReferenceUrl(_lQuillTenure.LTenureId, new LStateWritten(text)));
    }

    public void LQuillNoteSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(new LRequestReferenceNote(_lQuillTenure.LTenureId, new LStateWritten(text)));
    }

    public void LQuillKindSet(string? tag)
    {
        if (LReferenceClerk.LReferenceKindRead(_lQuillTenure.LTenureRead(), tag) is not LReferenceKind kind)
        {
            return;
        }

        _lQuillTenure.LTenureRequestApply(new LRequestReferenceKind(_lQuillTenure.LTenureId, kind));
    }

    public bool LQuillAuthorAdd(string? name, int position, long former)
    {
        if (!LDraftClerkPanel.LAuthorNameCheck(name))
        {
            return false;
        }

        _lQuillTenure.LTenureRequestApply(
            new LRequestAuthorAddition(_lQuillTenure.LTenureId, name, position, former));
        return true;
    }

    public bool LQuillAuthorInsert(long author, int position, long former)
    {
        if (!LDraftClerkList.LDraftFormerCheck(author, former))
        {
            return false;
        }

        _lQuillTenure.LTenureRequestApply(
            new LRequestAuthorPick(_lQuillTenure.LTenureId, author, position, former));
        return true;
    }

    public void LQuillAuthorRemove(long author)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestAuthorRemoval(_lQuillTenure.LTenureId, author));
    }

    public void LQuillAuthorMove(long author, int position)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestAuthorShift(_lQuillTenure.LTenureId, author, position));
    }
}
