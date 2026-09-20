using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LLanguageCache
{
    private readonly object _lLanguageCacheGate = new();
    private readonly LLanguageVault _lLanguageCacheVault;
    private readonly Dictionary<string, LLanguage> _lLanguageCachePacks = new(StringComparer.Ordinal);

    public LLanguageCache(LLanguageVault vault)
    {
        ArgumentNullException.ThrowIfNull(vault);
        _lLanguageCacheVault = vault;
    }

    public LLanguage LLanguageCacheRead(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        lock (_lLanguageCacheGate)
        {
            if (!_lLanguageCachePacks.TryGetValue(language, out LLanguage? pack))
            {
                pack = _lLanguageCacheVault.LLanguageRead(language);
                _lLanguageCachePacks[language] = pack;
            }

            return pack;
        }
    }

    public LPronunciationDraft LLanguageRespellingResolve(string language, LPronunciationDraft spoken)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(spoken);

        IReadOnlyList<LRespelling> groups = language.Trim().Length == 0
            ? []
            : LLanguageCacheRead(language).LLanguageRespellings;
        string respelling = groups.Count == 0 || spoken.LPronunciationDraftIpa.Length == 0
            ? string.Empty
            : LRespelling.LRespellingScan(
                groups, spoken.LPronunciationDraftIpa, spoken.LPronunciationDraftVariety.Trim());
        return spoken with { LPronunciationDraftRespelling = respelling };
    }

    public LReflexDraft LLanguageRespellingResolve(LReflexDraft row)
    {
        ArgumentNullException.ThrowIfNull(row);

        string language = row.LReflexDraftLanguage.Trim();
        LLanguage? pack = language.Length == 0 ? null : LLanguageCacheRead(language);
        if (pack is null || pack.LLanguageRespellings.Count == 0 || row.LReflexDraftText.Trim().Length == 0)
        {
            return row with { LReflexDraftRespelling = string.Empty };
        }

        return row with
        {
            LReflexDraftRespelling =
                LRespelling.LRespellingScan(pack.LLanguageRespellings, row.LReflexDraftText, string.Empty),
        };
    }

    public LEntryDraft LLanguageRespellingRebuild(LEntryDraft content)
    {
        ArgumentNullException.ThrowIfNull(content);

        List<LPronunciationDraft> spoken = new(content.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft row in content.LEntryDraftPronunciations)
        {
            spoken.Add(LLanguageRespellingResolve(content.LEntryDraftLanguage, row));
        }

        return content with { LEntryDraftPronunciations = spoken };
    }

    public LEntryDraft LLanguageRespellingUpdate(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        List<LPronunciationDraft> spoken = new(draft.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft row in draft.LEntryDraftPronunciations)
        {
            spoken.Add(row.LPronunciationDraftRespelling.Length == 0
                ? LLanguageRespellingResolve(draft.LEntryDraftLanguage, row)
                : row);
        }

        List<LReflexDraft> reflexes = new(draft.LEntryDraftReflexes.Count);
        foreach (LReflexDraft row in draft.LEntryDraftReflexes)
        {
            reflexes.Add(LLanguageAnatomyResolve(
                draft.LEntryDraftLanguage,
                row.LReflexDraftRespelling.Length == 0 ? LLanguageRespellingResolve(row) : row));
        }

        return draft with { LEntryDraftPronunciations = spoken, LEntryDraftReflexes = reflexes };
    }

    public LReflexDraft LLanguageAnatomyResolve(string language, LReflexDraft row)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(row);

        IReadOnlyList<LAnatomyRule> rules = language.Trim().Length == 0
            ? []
            : LLanguageCacheRead(language).LLanguageAnatomies;
        LAnatomy anatomy = rules.Count == 0
            ? LAnatomy.LAnatomyEmpty
            : LAnatomy.LAnatomyScan(
                rules, row.LReflexDraftLanguage, row.LReflexDraftText, row.LReflexDraftRespelling);
        return row with { LReflexDraftAnatomy = anatomy };
    }

    public IReadOnlyList<LReflexDraft> LLanguageAnatomyScan(string language, IReadOnlyList<LReflexDraft> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<LReflexDraft> filled = new(rows.Count);
        foreach (LReflexDraft row in rows)
        {
            filled.Add(LLanguageAnatomyResolve(language, row));
        }

        return filled;
    }

    public LEntryDraft LLanguageAnatomyRebuild(LEntryDraft content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return content with
        {
            LEntryDraftReflexes = LLanguageAnatomyScan(content.LEntryDraftLanguage, content.LEntryDraftReflexes),
        };
    }
}
