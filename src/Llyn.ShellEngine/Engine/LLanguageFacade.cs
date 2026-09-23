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

    public LFont LEngineFontRead(string language)
    {
        return LEngineFontRead(language, LFontRole.LFontRoleHeadword);
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

    public async Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad()
    {
        string[] missing = LLanguageFacadeStaff.LEngineStaffEnsign.LEnsignMissingRead(
            LEngineLanguageRead(), out int age);
        if (missing.Length == 0)
        {
            return [];
        }

        string?[] paths = await Task.WhenAll(missing.Select(LEngineEnsignRead)).ConfigureAwait(false);
        return LLanguageFacadeStaff.LEngineStaffEnsign.LEnsignPathAdd(age, missing, paths);
    }

    public async Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad(string language, IEnumerable<string> varieties)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentNullException.ThrowIfNull(varieties);

        Dictionary<string, string> keyed = new(StringComparer.Ordinal);
        foreach (string variety in varieties)
        {
            keyed[LEnsign.LEnsignKeyFormat(language, variety)] = variety;
        }

        string[] missing = LLanguageFacadeStaff.LEngineStaffEnsign.LEnsignMissingRead(keyed.Keys, out int age);
        if (missing.Length == 0)
        {
            return [];
        }

        string?[] paths = await Task
            .WhenAll(missing.Select(key => LEngineEnsignResolve(language, keyed[key])))
            .ConfigureAwait(false);
        return LLanguageFacadeStaff.LEngineStaffEnsign.LEnsignPathAdd(age, missing, paths);
    }

    public void LEngineEnsignDelete(string path)
    {
        LLanguageFacadeStaff.LEngineStaffEnsign.LEnsignPathDelete(path);
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

    public IReadOnlyList<LScriptImage> LEngineScriptRead(long entryId)
    {
        return LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkRead(entryId);
    }

    public void LEngineScriptStart(long entryId)
    {
        LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkStart(entryId);
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

    public bool LEngineScriptCheck(long entryId)
    {
        return LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkCheck(entryId);
    }

    internal Task<IReadOnlyList<LScriptImage>> LEngineScriptFind(
        string character, string language, CancellationToken cancellation)
    {
        return LLanguageFacadeStaff.LEngineStaffScript.LScriptClerkFind(character, language, cancellation);
    }

    private LEngineStaff LLanguageFacadeStaff => _lLanguageFacadeEngine.LEngineStaffHeld;
}
