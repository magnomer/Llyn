using System;
using System.Collections.Generic;
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
            return LLanguageLoader.LLanguageLoaderScan();
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
            _ => pack.LLanguageFont,
        };
    }

    public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)
    {
        string? code = LEngineLanguageLoad(language).LLanguageFlag;
        return await LEngineFlagResolve(code, cancellation).ConfigureAwait(false);
    }

    public IReadOnlyList<LVariety> LEngineVarietyRead(string language)
    {
        return LEngineLanguageLoad(language).LLanguageVarieties;
    }

    public bool LEngineFlaggedCheck(string language)
    {
        return LEngineLanguageLoad(language).LLanguageVarietyFlagged;
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
