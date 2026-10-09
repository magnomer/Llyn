using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LLanguageFacade : LGlyphPort, LLanguagePort
{
    private readonly LEngineHearth _lLanguageFacadeHearth;
    private readonly LFanqieFacade _lLanguageFacadeFanqie;
    private readonly LReflexFacade _lLanguageFacadeReflex;
    private readonly LScriptFacade _lLanguageFacadeScript;
    private readonly LSettingsFacade _lLanguageFacadeSettings;
    private readonly LVocabularyFacade _lLanguageFacadeVocabulary;
    private readonly object _lLanguageFacadeGate;
    private readonly SemaphoreSlim _lLanguageFacadeEnsign = new(1, 1);

    internal LLanguageFacade(
        LEngineHearth hearth,
        LFanqieFacade fanqie,
        LReflexFacade reflex,
        LScriptFacade script,
        LSettingsFacade settings,
        LVocabularyFacade vocabulary)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(fanqie);
        ArgumentNullException.ThrowIfNull(reflex);
        ArgumentNullException.ThrowIfNull(script);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(vocabulary);
        _lLanguageFacadeHearth = hearth;
        _lLanguageFacadeFanqie = fanqie;
        _lLanguageFacadeReflex = reflex;
        _lLanguageFacadeScript = script;
        _lLanguageFacadeSettings = settings;
        _lLanguageFacadeVocabulary = vocabulary;
        _lLanguageFacadeGate = _lLanguageFacadeHearth.LEngineGate;
    }

    public IReadOnlyList<string> LEngineLanguageRead()
    {
        lock (_lLanguageFacadeGate)
        {
            return LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageClerkRead();
        }
    }

    public string LEngineGlossRead()
    {
        string gloss = _lLanguageFacadeHearth.LEngineSettingsRead().LSettingsGloss;
        IReadOnlyList<string> languages = LEngineLanguageRead();
        if (languages.Contains(gloss, StringComparer.Ordinal))
        {
            return gloss;
        }

        return languages.Count > 0 ? languages[0] : string.Empty;
    }

    public LFont LEngineFontRead(string language, LFontRole role)
    {
        LLanguage pack = LEngineLanguageLoad(language);

        return role switch
        {
            LFontRole.LFontRoleExample => pack.LLanguageExample,
            LFontRole.LFontRoleGloss => pack.LLanguageGloss,
            LFontRole.LFontRoleGlyph => pack.LLanguageGlyph?.LGlyphFont ?? pack.LLanguageExample,
            _ => pack.LLanguageFont,
        };
    }

    public Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageFlagRead(language, cancellation);
    }

    public async Task<IReadOnlyList<string>> LEngineEnsignLoad(
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        await _lLanguageFacadeEnsign.WaitAsync().ConfigureAwait(true);
        try
        {
            LEnsign ensign = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffEnsign;
            IReadOnlyList<string> languages = LEngineLanguageRead();
            string[] missing = ensign.LEnsignMissingRead(languages, out int age);
            if (missing.Length == 0)
            {
                return languages;
            }

            string?[] paths = await Task
                .WhenAll(missing.Select(language => LEngineFlagRead(language, CancellationToken.None)))
                .ConfigureAwait(true);
            ensign.LEnsignPathAdd(age, missing, paths, store);
            return languages;
        }
        finally
        {
            _lLanguageFacadeEnsign.Release();
        }
    }

    public async Task LEngineEnsignLoad(
        string language,
        IEnumerable<string> varieties,
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentNullException.ThrowIfNull(varieties);

        Dictionary<string, string> keyed = new(StringComparer.Ordinal);
        foreach (string variety in varieties)
        {
            keyed[LEnsign.LEnsignKeyFormat(language, variety)] = variety;
        }

        await _lLanguageFacadeEnsign.WaitAsync().ConfigureAwait(true);
        try
        {
            LEnsign ensign = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffEnsign;
            string[] missing = ensign.LEnsignMissingRead(keyed.Keys, out int age);
            if (missing.Length == 0)
            {
                return;
            }

            string?[] paths = await Task
                .WhenAll(missing.Select(key => LEngineVarietyResolve(language, keyed[key], CancellationToken.None)))
                .ConfigureAwait(true);
            ensign.LEnsignPathAdd(age, missing, paths, store);
        }
        finally
        {
            _lLanguageFacadeEnsign.Release();
        }
    }

    internal IReadOnlyList<LVariety> LEngineVarietyRead(string language)
    {
        return LEngineLanguageLoad(language).LLanguageVarieties;
    }

    internal bool LEngineFlaggedCheck(string language)
    {
        return LEngineLanguageLoad(language).LLanguageVarietyFlagged;
    }

    internal bool LEngineSpacedCheck(string language)
    {
        return language.Length == 0 || LEngineLanguageLoad(language).LLanguageSpaced;
    }

    public bool LEngineFlaggedCheck(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        return draft.LEntryDraftLanguage.Length > 0 && LEngineFlaggedCheck(draft.LEntryDraftLanguage);
    }

    public LAccentSheet LEngineAccentRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LEngineAccentRead(
            draft, draft.LEntryDraftAccents.Where(static spoken => spoken.LPronunciationDraftNotated));
    }

    public LAccentSheet LEngineAccentRead(LEntryDraft draft, IEnumerable<LPronunciationDraft> accents)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(accents);

        string language = draft.LEntryDraftLanguage;
        (bool respelled, string opener, string closer) =
            _lLanguageFacadeSettings.LEngineMarkRead(language);
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        LAccentRow primary = languages.LLanguageAccentRead(draft.LEntryDraftPronunciation, respelled);
        return new LAccentSheet(
            language,
            LEngineFlaggedCheck(draft),
            languages.LLanguageContourRead(language, primary.LAccentRowText),
            respelled,
            opener,
            closer,
            primary,
            accents.Select(spoken => languages.LLanguageAccentRead(spoken, respelled)).ToList());
    }

    public async Task<LAccentSheet> LEngineAccentLoad(
        LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        LAccentSheet sheet = LEngineAccentRead(draft);
        await LEngineEnsignLoad(sheet, store).ConfigureAwait(true);
        return sheet;
    }

    public async Task LEngineEnsignLoad(
        LAccentSheet sheet, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        if (sheet.LAccentSheetFlagged)
        {
            await LEngineEnsignLoad(sheet.LAccentSheetLanguage, sheet.LAccentSheetVarieties, store)
                .ConfigureAwait(true);
        }
    }

    public IReadOnlyList<LContour> LEngineContourRead(string language, string ipa)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageContourRead(language, ipa);
    }

    public bool LEngineSilentCheck(string language)
    {
        return !string.IsNullOrWhiteSpace(language) && LEngineLanguageLoad(language).LLanguageSilent;
    }

    public Task<string?> LEngineVarietyResolve(string language, string variety, CancellationToken cancellation)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LVarietyFlagRead(language, variety, cancellation);
    }

    internal LLanguage LEngineLanguageLoad(string language)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageClerkLoad(language);
    }

    public IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageTranscriptionRead(draft);
    }

    public LGlyphBlock LEngineGlyphRead(LEntryDraft draft)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageGlyphRead(draft);
    }

    LGlyph? LGlyphPort.LEngineGlyphRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LEngineGlyphRead(draft.LEntryDraftLanguage);
    }

    public LGlyph? LEngineGlyphRead(string language)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageGlyphLoad(language);
    }

    public IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageGlyphDivide(draft);
    }

    public IReadOnlyList<int> LEngineContourScale => LLanguageClerk.LLanguageContourScale;

    public void LEngineSoundStart(long entryId)
    {
        _lLanguageFacadeReflex.LEngineReflexStart(entryId);
        _lLanguageFacadeVocabulary.LEngineInflectionStart(entryId);
        _lLanguageFacadeScript.LEngineScriptStart(entryId);
        _lLanguageFacadeFanqie.LEngineFanqieStart(entryId);
    }

    private LEngineStaff LLanguageFacadeStaff => _lLanguageFacadeHearth.LEngineStaffHeld;
}
