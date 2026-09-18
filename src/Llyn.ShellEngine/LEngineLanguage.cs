using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<string> LEngineLanguageRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineLanguageListed ??= LLanguageLoader.LLanguageLoaderScan();
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
        return LEngineLanguageLoad(language).LLanguageTonal;
    }

    public bool LEngineSilentCheck(string language)
    {
        return LEngineLanguageLoad(language).LLanguageSilent;
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

        string workspace;
        lock (_lEngineGate)
        {
            workspace = _lEngineWorkspace;
        }

        return await LWorkspace
            .LWorkspaceFlagRead(code, workspace, _lEngineClient, cancellation)
            .ConfigureAwait(false);
    }

    private LLanguage LEngineLanguageLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        lock (_lEngineGate)
        {
            if (!_lEngineLanguages.TryGetValue(language, out LLanguage? pack))
            {
                pack = LLanguageLoader.LLanguageLoaderLoad(language);
                _lEngineLanguages[language] = pack;
            }

            return pack;
        }
    }
}
