using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LLanguageClerk
{
    private readonly LLanguageVault _lLanguageClerkVault;
    private readonly LLanguageCache _lLanguageClerkLanguages;
    private readonly LTrailClerk _lLanguageClerkTrail;
    private IReadOnlyList<string>? _lLanguageClerkListed;

    public LLanguageClerk(LRig rig, LLanguageCache languages, LTrailClerk trail)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(trail);
        _lLanguageClerkVault = rig.LRigLanguages;
        _lLanguageClerkLanguages = languages;
        _lLanguageClerkTrail = trail;
    }

    public IReadOnlyList<string> LLanguageClerkRead()
    {
        return _lLanguageClerkListed ??= _lLanguageClerkVault.LLanguageScan();
    }

    public LLanguage LLanguageClerkLoad(string language)
    {
        return _lLanguageClerkLanguages.LLanguageCacheRead(language);
    }

    public bool LLanguageRespellingCheck(string language, bool respelled)
    {
        ArgumentNullException.ThrowIfNull(language);
        return respelled
            && language.Trim().Length > 0
            && LLanguageClerkLoad(language).LLanguageRespellings.Count > 0;
    }

    public bool LLanguagePhonemicCheck(string language)
    {
        ArgumentNullException.ThrowIfNull(language);
        return language.Trim().Length > 0 && LLanguageClerkLoad(language).LLanguagePhonemic;
    }

    public Task<string?> LLanguageFlagRead(string language, CancellationToken cancellation)
    {
        return LLanguageFlagResolve(LLanguageClerkLoad(language).LLanguageFlag, cancellation);
    }

    public Task<string?> LVarietyFlagRead(string language, string variety, CancellationToken cancellation)
    {
        string? code = null;
        foreach (LVariety declared in LLanguageClerkLoad(language).LLanguageVarieties)
        {
            if (string.Equals(declared.LVarietyName, variety, StringComparison.Ordinal))
            {
                code = declared.LVarietyFlag;
                break;
            }
        }

        return LLanguageFlagResolve(code, cancellation);
    }

    private async Task<string?> LLanguageFlagResolve(string? code, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        if (_lLanguageClerkTrail.LTrailRootCheck(code))
        {
            return _lLanguageClerkTrail.LTrailPathCheck(code) ? code : null;
        }

        return await _lLanguageClerkVault.LLanguageFlagRead(code, cancellation).ConfigureAwait(false);
    }
}
