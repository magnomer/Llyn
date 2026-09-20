using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<string> LEngineLanguageRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineLanguageClerk.LLanguageClerkRead();
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
        lock (_lEngineGate)
        {
            languages = _lEngineLanguageClerk;
        }

        return languages.LLanguageFlagRead(language, cancellation);
    }

    public async Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad()
    {
        string[] missing = _lEngineEnsign.LEnsignMissingRead(LEngineLanguageRead(), out int age);
        if (missing.Length == 0)
        {
            return [];
        }

        string?[] paths = await Task.WhenAll(missing.Select(LEngineEnsignRead)).ConfigureAwait(false);
        return _lEngineEnsign.LEnsignPathAdd(age, missing, paths);
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

        string[] missing = _lEngineEnsign.LEnsignMissingRead(keyed.Keys, out int age);
        if (missing.Length == 0)
        {
            return [];
        }

        string?[] paths = await Task
            .WhenAll(missing.Select(key => LEngineEnsignResolve(language, keyed[key])))
            .ConfigureAwait(false);
        return _lEngineEnsign.LEnsignPathAdd(age, missing, paths);
    }

    public void LEngineEnsignDelete(string path)
    {
        _lEngineEnsign.LEnsignPathDelete(path);
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
        lock (_lEngineGate)
        {
            languages = _lEngineLanguageClerk;
        }

        return languages.LVarietyFlagRead(language, variety, cancellation);
    }

    private LLanguage LEngineLanguageLoad(string language)
    {
        LLanguageClerk languages;
        lock (_lEngineGate)
        {
            languages = _lEngineLanguageClerk;
        }

        return languages.LLanguageClerkLoad(language);
    }

    public IReadOnlyList<LScriptStyle> LEngineStyleRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineScriptClerk.LScriptStyleRead(language);
        }
    }

    public IReadOnlyList<LScriptImage> LEngineScriptRead(long entryId)
    {
        return _lEngineScriptClerk.LScriptClerkRead(entryId);
    }

    public void LEngineScriptStart(long entryId)
    {
        _lEngineScriptClerk.LScriptClerkStart(entryId);
    }

    public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId)
    {
        return _lEngineScriptClerk.LScriptClerkDivide(entryId);
    }

    public bool LEngineScriptCheck(long entryId)
    {
        return _lEngineScriptClerk.LScriptClerkCheck(entryId);
    }

    internal Task<IReadOnlyList<LScriptImage>> LEngineScriptFind(
        string character, string language, CancellationToken cancellation)
    {
        return _lEngineScriptClerk.LScriptClerkFind(character, language, cancellation);
    }
}
