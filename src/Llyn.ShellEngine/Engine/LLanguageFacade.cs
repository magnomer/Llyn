using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LLanguageFacade
{
    private readonly LEngine _lLanguageFacadeEngine;
    private readonly object _lLanguageFacadeGate;

    public LLanguageFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lLanguageFacadeEngine = engine;
        _lLanguageFacadeGate = engine.LEngineGate;
    }

    public IReadOnlyList<string> LEngineLanguageRead()
    {
        lock (_lLanguageFacadeGate)
        {
            return LLanguageFacadeStaff.LEngineStaffLanguage.LLanguageClerkRead();
        }
    }

    public string LEngineGlossRead()
    {
        string gloss = _lLanguageFacadeEngine.LEngineSettingsRead().LSettingsGloss;
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
            languages = LLanguageFacadeStaff.LEngineStaffLanguage;
        }

        return languages.LLanguageFlagRead(language, cancellation);
    }

    public async Task LEngineEnsignLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        LEnsign ensign = LLanguageFacadeStaff.LEngineStaffEnsign;
        string[] missing = ensign.LEnsignMissingRead(LEngineLanguageRead(), out int age);
        if (missing.Length == 0)
        {
            return;
        }

        string?[] paths = await Task.WhenAll(missing.Select(LEngineEnsignRead)).ConfigureAwait(true);
        ensign.LEnsignPathAdd(age, missing, paths, store);
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

        LEnsign ensign = LLanguageFacadeStaff.LEngineStaffEnsign;
        string[] missing = ensign.LEnsignMissingRead(keyed.Keys, out int age);
        if (missing.Length == 0)
        {
            return;
        }

        string?[] paths = await Task
            .WhenAll(missing.Select(key => LEngineEnsignResolve(language, keyed[key])))
            .ConfigureAwait(true);
        ensign.LEnsignPathAdd(age, missing, paths, store);
    }

    private async Task<string?> LEngineEnsignRead(string language)
    {
        try
        {
            return await LEngineFlagRead(language, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<string?> LEngineEnsignResolve(string language, string variety)
    {
        try
        {
            return await LEngineVarietyResolve(language, variety, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return null;
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

    public bool LEngineFlaggedCheck(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        return draft.LEntryDraftLanguage.Length > 0 && LEngineFlaggedCheck(draft.LEntryDraftLanguage);
    }

    public LAccentSheet LEngineAccentRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        string language = draft.LEntryDraftLanguage;
        (bool respelled, string opener, string closer) =
            _lLanguageFacadeEngine.LEngineSettings.LEngineMarkRead(language);
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage;
        }

        return new LAccentSheet(
            language,
            LEngineFlaggedCheck(draft),
            LEngineTonalCheck(language),
            respelled,
            opener,
            closer,
            languages.LLanguageAccentRead(draft.LEntryDraftPronunciation, respelled),
            draft.LEntryDraftAccents
                .Where(static spoken => spoken.LPronunciationDraftNotated)
                .Select(spoken => languages.LLanguageAccentRead(spoken, respelled))
                .ToList());
    }

    public async Task<LAccentSheet> LEngineAccentLoad(
        LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)
    {
        LAccentSheet sheet = LEngineAccentRead(draft);
        if (sheet.LAccentSheetFlagged)
        {
            await LEngineEnsignLoad(sheet.LAccentSheetLanguage, sheet.LAccentSheetVarieties, store)
                .ConfigureAwait(true);
        }

        return sheet;
    }

    public bool LEngineTonalCheck(string language)
    {
        return !string.IsNullOrWhiteSpace(language) && LEngineLanguageLoad(language).LLanguageTonal;
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
            languages = LLanguageFacadeStaff.LEngineStaffLanguage;
        }

        return languages.LVarietyFlagRead(language, variety, cancellation);
    }

    internal LLanguage LEngineLanguageLoad(string language)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage;
        }

        return languages.LLanguageClerkLoad(language);
    }

    public IReadOnlyList<LScriptStyle> LEngineStyleRead(string language)
    {
        lock (_lLanguageFacadeGate)
        {
            return LLanguageFacadeStaff.LEngineStaffScript.LScriptStyleRead(language);
        }
    }

    public void LEngineScriptStart(long entryId)
    {
        LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkStart(entryId);
    }

    public IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft)
    {
        LLanguageClerk languages;
        lock (_lLanguageFacadeGate)
        {
            languages = LLanguageFacadeStaff.LEngineStaffLanguage;
        }

        return languages.LLanguageTranscriptionRead(draft);
    }

    public void LEngineSoundStart(long entryId)
    {
        _lLanguageFacadeEngine.LEngineReflex.LEngineReflexStart(entryId);
        _lLanguageFacadeEngine.LEngineVocabulary.LEngineInflectionStart(entryId);
        LEngineScriptStart(entryId);
        _lLanguageFacadeEngine.LEngineFanqie.LEngineFanqieStart(entryId);
    }

    public bool LEngineStyleCheck(string language)
    {
        return LEngineStyleRead(language).Count > 0;
    }

    public void LEngineScriptRebuild(long entryId)
    {
        LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkRebuild(entryId);
    }

    public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId)
    {
        return LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkDivide(entryId);
    }

    public IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId)
    {
        LEngineScriptStart(entryId);
        return LEngineScriptDivide(entryId);
    }

    public bool LEngineScriptCheck(long entryId)
    {
        return LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkCheck(entryId);
    }

    private LEngineStaff LLanguageFacadeStaff => _lLanguageFacadeEngine.LEngineStaffHeld;
}
