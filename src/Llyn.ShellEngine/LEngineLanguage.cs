using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
            return _lEngineLanguageListed ??= _lEngineLanguageVault.LLanguageScan();
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

    public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)
    {
        string? code = LEngineLanguageLoad(language).LLanguageFlag;
        return await LEngineFlagResolve(code, cancellation).ConfigureAwait(false);
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

    public async Task<string?> LEngineVarietyResolve(string language, string variety, CancellationToken cancellation)
    {
        string? code = null;
        foreach (LVariety declared in LEngineLanguageLoad(language).LLanguageVarieties)
        {
            if (string.Equals(declared.LVarietyName, variety, StringComparison.Ordinal))
            {
                code = declared.LVarietyFlag;
                break;
            }
        }

        return await LEngineFlagResolve(code, cancellation).ConfigureAwait(false);
    }

    private async Task<string?> LEngineFlagResolve(string? code, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        if (Path.IsPathRooted(code))
        {
            return File.Exists(code) ? code : null;
        }

        LLanguageVault languages;
        lock (_lEngineGate)
        {
            languages = _lEngineLanguageVault;
        }

        return await languages.LLanguageFlagRead(code, cancellation).ConfigureAwait(false);
    }

    private LLanguage LEngineLanguageLoad(string language)
    {
        LLanguageCache languages;
        lock (_lEngineGate)
        {
            languages = _lEngineLanguageCache;
        }

        return languages.LLanguageCacheRead(language);
    }
}
