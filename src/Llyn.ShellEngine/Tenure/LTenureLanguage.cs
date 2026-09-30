using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    private string? _lTenureSpeechPending;

    public bool LTenureFlaggedCheck()
    {
        string language = LTenureLanguageRead();
        return language.Length > 0 && _lEngine.LEngineLanguage.LEngineFlaggedCheck(language);
    }

    public IReadOnlyList<LVariety> LTenureVarietyRead()
    {
        string language = LTenureLanguageRead();
        return language.Length == 0 ? [] : _lEngine.LEngineLanguage.LEngineVarietyRead(language);
    }

    public IReadOnlyList<string> LTenureVarietyNames =>
        LTenureVarietyRead().Select(static variety => variety.LVarietyName).ToList();

    public IReadOnlyList<LSchemeRow> LTenureSchemeRead(long transcription)
    {
        IReadOnlyList<LTranscriptionDraft> rows = LTenureRead()?.LDraftContent.LEntryDraftTranscriptions ?? [];
        return _lEngine.LEnginePronunciation.LEngineSchemeRead(LTenureLanguageRead())
            .Select(scheme => new LSchemeRow(scheme, LDraftClerkReading.LSchemeTakenCheck(rows, scheme, transcription)))
            .ToList();
    }

    public bool LTenureReflexCheck()
    {
        return _lEngine.LEngineReflex.LEngineReflexRead(LTenureLanguageRead()).Count > 0
            || LTenureRead()?.LDraftContent.LEntryDraftReflected == true;
    }

    public IReadOnlyList<LAnchorRow> LTenureAnchorScan(long reflex)
    {
        LDraft? held = LTenureRead();
        if (held?.LDraftStored is not long stored)
        {
            return [];
        }

        try
        {
            return LDraftClerkReflex.LReflexFind(held.LDraftContent, reflex) is LReflexDraft row
                ? _lEngine.LEngineReflex.LEngineAnchorScan(
                    stored,
                    row.LReflexDraftAnchors,
                    held.LDraftContent.LEntryDraftLanguage,
                    row.LReflexDraftLanguage,
                    row.LReflexDraftAnatomy.LAnatomyToneIpa)
                : [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public string LTenurePronunciationRead()
    {
        return LTenureRead()?.LDraftContent is LEntryDraft draft
            ? _lEngine.LEngineSettings.LEnginePronunciationRead(draft)
            : string.Empty;
    }

    public void LTenurePronunciationSet(string text)
    {
        if (_lEngine.LEngineSettings.LEngineRespellingCheck(LTenureLanguageRead()))
        {
            LTenureRespellingSet(text);
            return;
        }

        LTenureIpaSet(text);
    }

    public void LTenureIpaSet(string text)
    {
        LTenureRequestDefer(new LRequestIpa(LTenureId, text));
    }

    public void LTenureRespellingSet(string text)
    {
        LTenureRequestDefer(new LRequestRespelling(LTenureId, text));
    }

    public void LTenureVarietySet(long pronunciation, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        if (pronunciation == 0 || variety.Length == 0)
        {
            return;
        }

        LTenureRequestApply(new LRequestPronunciationVariety(LTenureId, pronunciation, variety));
    }

    public IReadOnlyList<LTranslationTarget> LTenureEtymonRead()
    {
        if (LTenureRead()?.LDraftContent is not LEntryDraft draft)
        {
            return [];
        }

        try
        {
            return _lEngine.LEngineCard.LEngineEtymonRead(draft);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LTenureTranslationRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        IReadOnlyList<LCardDraft> cards = [.. draft.LEntryDraftMeanings, .. draft.LEntryDraftCollocations];
        List<long> ids = cards.SelectMany(static card => card.LCardDraftTranslation).Distinct().ToList();
        IReadOnlyList<LTranslationTarget> read;
        try
        {
            read = ids.Count == 0 ? [] : _lEngine.LEngineCard.LEngineTargetRead(LTenureId, ids);
        }
        catch (Exception)
        {
            read = [];
        }

        return LCardFacade.LEngineTranslationResolve(cards, read);
    }

    public (IReadOnlyList<string> LSpeechNames, string LSpeechTyped, LSpeechOffer LSpeechFound) LTenureSpeechRead(
        string typed)
    {
        IReadOnlyList<LSpeechDraft> shown = LTenureRead()?.LDraftContent.LEntryDraftSpeeches ?? [];
        (IReadOnlyList<LSpeechDraft> held, string kept) =
            LSpeechClerk.LSpeechSettle(shown, LSpeechClerk.LSpeechChipRead(shown, _lTenureSpeechPending), typed);
        if (kept.Length == 0)
        {
            _lTenureSpeechPending = null;
        }

        return (held.Select(static speech => speech.LSpeechDraftName).ToList(), kept, LTenureSpeechFind(held, kept));
    }

    public LSpeechOffer LTenureSpeechSet(string typed)
    {
        IReadOnlyList<LSpeechDraft> held = LTenureChipRead();
        LTenureSpeechSend(held, typed, true);
        return LTenureSpeechFind(held, typed);
    }

    public void LTenureSpeechAdd(string name)
    {
        string language = LTenureLanguageRead();
        IReadOnlyList<LSpeechDraft>? added =
            LSpeechClerk.LSpeechAdd(LTenureChipRead(), name, typed => LTenureSpeechCreate(language, typed));
        if (added is null)
        {
            return;
        }

        LTenureSpeechSend(added, string.Empty, false);
    }

    public void LTenureSpeechRemove(string name, string typed)
    {
        LTenureSpeechSend(LSpeechClerk.LSpeechRemove(LTenureChipRead(), name), typed, false);
    }

    public void LTenureGlossAdd(long card, long sentence)
    {
        LTenureGlossInsert(card, sentence, int.MaxValue);
    }

    public void LTenureGlossInsert(long card, long sentence, int position)
    {
        LTenureRequestApply(new LRequestGlossAddition(
            LTenureId, card, sentence, _lEngine.LEngineLanguage.LEngineGlossRead(), position));
    }

    public bool LTenureGlossPrepare(long card, long sentence)
    {
        LDraft? held = LTenureRead();
        if (held is null || !LDraftClerkGloss.LGlossEmptyCheck(held, card, sentence))
        {
            return false;
        }

        LTenureGlossAdd(card, sentence);
        return true;
    }

    private IReadOnlyList<LSpeechDraft> LTenureChipRead()
    {
        IReadOnlyList<LSpeechDraft> shown = LTenureRead()?.LDraftContent.LEntryDraftSpeeches ?? [];
        return LSpeechClerk.LSpeechChipRead(shown, _lTenureSpeechPending);
    }

    private void LTenureSpeechSend(IReadOnlyList<LSpeechDraft> held, string typed, bool deferred)
    {
        _lTenureSpeechPending = LSpeechClerk.LSpeechPendingRead(held, typed);
        LRequestSpeech request = new(LTenureId, LSpeechClerk.LSpeechParse(held, typed));
        if (deferred)
        {
            LTenureRequestDefer(request);
            return;
        }

        LTenureRequestApply(request);
    }

    private LSpeechOffer LTenureSpeechFind(IReadOnlyList<LSpeechDraft> held, string typed)
    {
        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _lEngine.LEngineVocabulary.LEngineSpeechRead(LTenureLanguageRead());
        }
        catch (Exception)
        {
            values = [];
        }

        return LSpeechClerk.LSpeechFind(values, held, typed);
    }

    private LSpeechValue? LTenureSpeechCreate(string language, string name)
    {
        try
        {
            return _lEngine.LEngineVocabulary.LEngineSpeechAdd(language, name);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
