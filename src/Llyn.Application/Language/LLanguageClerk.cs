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

    public static IReadOnlyList<int> LLanguageContourScale => LContour.LContourScale;

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

    public string LLanguagePronunciationRead(LEntryDraft draft, bool respelled)
    {
        ArgumentNullException.ThrowIfNull(draft);
        return draft.LEntryDraftPronunciation?.LPronunciationDraftRead(
                LLanguageRespellingCheck(draft.LEntryDraftLanguage, respelled))
            ?? string.Empty;
    }
    public LAccentRow LLanguageAccentRead(LPronunciationDraft? spoken, bool respelled)
    {
        return spoken is null
            ? new LAccentRow(0, string.Empty, string.Empty, string.Empty)
            : new LAccentRow(
                spoken.LPronunciationDraftId,
                spoken.LPronunciationDraftVariety,
                spoken.LPronunciationDraftRead(respelled),
                spoken.LPronunciationDraftAudio);
    }

    public static string LLanguageCandidateRead(string? phonetic, string? respelling, bool respelled)
    {
        LPronunciationDraft spoken = new(
            phonetic ?? string.Empty, LPronunciationDraftRespelling: respelling ?? string.Empty);
        return spoken.LPronunciationDraftRead(respelled);
    }

    public IReadOnlyList<LTranscriptionDraft> LLanguageTranscriptionRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LGlyph.LGlyphOtherRead(LLanguageGlyphLoad(draft.LEntryDraftLanguage), draft.LEntryDraftTranscriptions);
    }

    public LGlyphBlock LLanguageGlyphRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LGlyph.LGlyphBlockRead(LLanguageGlyphLoad(draft.LEntryDraftLanguage), draft.LEntryDraftTranscriptions);
    }

    public IReadOnlyList<LGlyphCell> LLanguageGlyphDivide(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LLanguageGlyphLoad(draft.LEntryDraftLanguage)?.LGlyphDivide(draft) ?? [];
    }

    public LGlyph? LLanguageGlyphLoad(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? null : LLanguageClerkLoad(language).LLanguageGlyph;
    }

    public bool LLanguagePhonemicCheck(string language)
    {
        ArgumentNullException.ThrowIfNull(language);
        return language.Trim().Length > 0 && LLanguageClerkLoad(language).LLanguagePhonemic;
    }

    public IReadOnlyList<LContour> LLanguageContourRead(string language, string ipa)
    {
        ArgumentNullException.ThrowIfNull(language);
        if (language.Trim().Length == 0 || !LLanguageClerkLoad(language).LLanguageTonal)
        {
            return [];
        }

        IReadOnlyList<LContour> syllables = LContour.LContourParse(ipa ?? string.Empty);
        return LContour.LContourToneCheck(syllables) ? syllables : [];
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
