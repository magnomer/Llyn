using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillPronunciation
{
    private readonly LTenure _lQuillPronunciationTenure;

    public LQuillPronunciation(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillPronunciationTenure = tenure;
    }

    public bool LQuillFlaggedCheck()
    {
        string language = _lQuillPronunciationTenure.LTenureLanguageRead();
        return language.Length > 0
            && _lQuillPronunciationTenure.LTenureEngine.LEngineLanguage.LEngineFlaggedCheck(language);
    }

    public LGlyphBlock? LQuillGlyphRead()
    {
        return _lQuillPronunciationTenure.LTenureRead()?.LDraftContent is LEntryDraft content
            ? _lQuillPronunciationTenure.LTenureEngine.LEngineLanguage.LEngineGlyphRead(content)
            : null;
    }

    public string LQuillPronunciationRead()
    {
        return _lQuillPronunciationTenure.LTenureRead()?.LDraftContent is LEntryDraft draft
            ? _lQuillPronunciationTenure.LTenureEngine.LEngineSettings.LEnginePronunciationRead(draft)
            : string.Empty;
    }

    public LAccentSheet? LQuillAccentRead()
    {
        LEngine engine = _lQuillPronunciationTenure.LTenureEngine;
        return _lQuillPronunciationTenure.LTenureRead()?.LDraftContent is LEntryDraft draft
            ? engine.LEngineLanguage.LEngineAccentRead(draft, draft.LEntryDraftAccents)
            : null;
    }

    public async Task<LAccentSheet?> LQuillAccentLoad(
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        LEngine engine = _lQuillPronunciationTenure.LTenureEngine;
        if (_lQuillPronunciationTenure.LTenureRead()?.LDraftContent is not LEntryDraft draft
            || !engine.LEngineLanguage.LEngineFlaggedCheck(draft))
        {
            return null;
        }

        LAccentSheet sheet = engine.LEngineLanguage.LEngineAccentRead(draft, draft.LEntryDraftAccents);
        await engine.LEngineLanguage.LEngineEnsignLoad(sheet, store).ConfigureAwait(true);
        return LQuillAccentRead() is LAccentSheet fresh
            && string.Equals(fresh.LAccentSheetLanguage, sheet.LAccentSheetLanguage, StringComparison.Ordinal)
                ? fresh
                : null;
    }

    public void LQuillPronunciationSet(string text)
    {
        if (_lQuillPronunciationTenure.LTenureEngine.LEngineSettings.LEngineRespellingCheck(
                _lQuillPronunciationTenure.LTenureLanguageRead()))
        {
            LQuillRespellingSet(text);
            return;
        }

        LQuillIpaSet(text);
    }

    public void LQuillAccentSet(long accent, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        long id = _lQuillPronunciationTenure.LTenureId;
        bool respelled = _lQuillPronunciationTenure.LTenureEngine.LEngineSettings.LEngineRespellingCheck(
            _lQuillPronunciationTenure.LTenureLanguageRead());
        _lQuillPronunciationTenure.LTenureRequestDefer(respelled
            ? new LRequestPronunciationRespelling(id, accent, text)
            : new LRequestPronunciationIpa(id, accent, text));
    }

    public void LQuillPronunciationAdd(long pronunciation)
    {
        IReadOnlyList<LPronunciationDraft> spoken =
            _lQuillPronunciationTenure.LTenureRead()?.LDraftContent.LEntryDraftPronunciations ?? [];
        _lQuillPronunciationTenure.LTenureRequestApply(new LRequestPronunciationAddition(
            _lQuillPronunciationTenure.LTenureId,
            string.Empty,
            LDraftClerkReading.LPronunciationPositionRead(spoken, pronunciation)));
    }

    public void LQuillPronunciationRemove(long pronunciation)
    {
        _lQuillPronunciationTenure.LTenureRequestApply(
            new LRequestPronunciationRemoval(_lQuillPronunciationTenure.LTenureId, pronunciation));
    }

    public Uri? LQuillAudioResolve(long pronunciation)
    {
        IReadOnlyList<LPronunciationDraft> spoken =
            _lQuillPronunciationTenure.LTenureRead()?.LDraftContent.LEntryDraftPronunciations ?? [];
        int index = pronunciation == 0
            ? -1
            : LDraftClerkList.LDraftListFind(spoken, pronunciation, static row => row.LPronunciationDraftId);
        if (index < 0)
        {
            return null;
        }

        Uri? address = _lQuillPronunciationTenure.LTenureEngine.LEnginePronunciation.LEngineAudioResolve(
            spoken[index].LPronunciationDraftAudio);
        if (address is null)
        {
            _lQuillPronunciationTenure.LTenureRequestApply(new LRequestPronunciationAudio(
                _lQuillPronunciationTenure.LTenureId, pronunciation, string.Empty, null));
        }

        return address;
    }

    public void LQuillIpaSet(string text)
    {
        _lQuillPronunciationTenure.LTenureRequestDefer(new LRequestIpa(_lQuillPronunciationTenure.LTenureId, text));
    }

    public void LQuillRespellingSet(string text)
    {
        _lQuillPronunciationTenure.LTenureRequestDefer(
            new LRequestRespelling(_lQuillPronunciationTenure.LTenureId, text));
    }

    public void LQuillVarietySet(bool primary, long pronunciation, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        long target = primary
            ? _lQuillPronunciationTenure.LTenureRead()?.LDraftContent
                .LEntryDraftPronunciation?.LPronunciationDraftId ?? 0
            : pronunciation;
        if (target == 0 || variety.Length == 0)
        {
            return;
        }

        _lQuillPronunciationTenure.LTenureRequestApply(
            new LRequestPronunciationVariety(_lQuillPronunciationTenure.LTenureId, target, variety));
    }
}
