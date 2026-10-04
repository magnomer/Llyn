using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReflexClerk
{
    private readonly LEntryVault _lReflexClerkEntries;
    private readonly LReflexVault _lReflexClerkReflexes;
    private readonly LLanguageCache _lReflexClerkLanguages;

    public LReflexClerk(
        LRig rig, LLanguageCache languages, LClaimClerk claims, object gate, Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        _lReflexClerkEntries = rig.LRigEntries;
        _lReflexClerkReflexes = rig.LRigReflexes;
        _lReflexClerkLanguages = languages;
        LReflexClerkFetch = new LReflexFetch(rig, this, languages, claims, gate, raise);
    }

    public LReflexFetch LReflexClerkFetch { get; }

    public static IReadOnlyList<LAnchorRow> LReflexAnchorScan(
        IReadOnlyList<LFanqieRow> rows,
        IReadOnlyList<long> anchors,
        IReadOnlyList<LDescent> tones,
        string reflex,
        string tone)
    {
        return LAnchor.LAnchorRowScan(rows, anchors, LDescent.LDescentScan(tones, reflex, tone));
    }

    public static bool LReflexAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        return LAnchor.LAnchorCheck(rows, headword);
    }

    public static string LReflexAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return LAnchor.LAnchorTextFormat(rows, anchors, headword, separator);
    }

    public IReadOnlyList<LReflexRule> LReflexRuleRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lReflexClerkLanguages.LLanguageCacheRead(language).LLanguageReflexRules;
    }

    public IReadOnlyList<string> LReflexFoldedRead(string language)
    {
        return LReflexRuleRead(language)
            .Where(static rule => rule.LReflexRuleFolded)
            .Select(static rule => rule.LReflexRuleLanguage)
            .ToList();
    }

    public void LReflexClerkSync(
        long entryId,
        string language,
        IReadOnlyList<LReflexDraft> drafts,
        List<LRevisionDelta> changes,
        Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(changes);
        ArgumentNullException.ThrowIfNull(identity);

        LReflexVault reflexes = _lReflexClerkReflexes;
        IReadOnlyList<LReflex> stored = reflexes.LReflexRead(entryId);
        IReadOnlyList<LReflexDraft> written = _lReflexClerkLanguages.LLanguageAnatomyScan(
            language, LDraftClerkEquality.LReflexScan(drafts));
        IReadOnlyList<LReflex> current = LReflexClerkRow.LReflexRowRead(entryId, written);

        if (LReflexClerkRow.LReflexRowMatch(stored, current))
        {
            return;
        }

        foreach (LReflex row in current)
        {
            if (row.LReflexId > 0 && !stored.Any(kept => kept.LReflexId == row.LReflexId))
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }
        }

        IReadOnlyList<LReflex> saved = reflexes.LReflexSet(entryId, current);
        LReflexEpithetSave(entryId);
        for (int index = 0; index < saved.Count; index++)
        {
            LIdentity.LIdentityRecord(identity, written[index].LReflexDraftId, saved[index].LReflexId);
        }

        if (LReflexClerkRow.LReflexRowMatch(
                LReflexClerkRow.LReflexAnatomyClear(stored), LReflexClerkRow.LReflexAnatomyClear(current)))
        {
            return;
        }

        changes.Add(new LRevisionDelta(
            entryId,
            "reflex",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            LReflexClerkRow.LReflexRowFormat(current)));
    }

    public static IReadOnlyList<LReflexDraft> LReflexClerkReset(IReadOnlyList<LReflexDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LReflexDraft> renewed = new(drafts.Count);
        foreach (LReflexDraft draft in drafts)
        {
            renewed.Add(draft.LReflexDraftId > 0 ? draft with { LReflexDraftId = 0 } : draft);
        }

        return renewed;
    }

    public void LReflexEpithetSave(long entryId)
    {
        LEntry? entry = _lReflexClerkEntries.LEntryRead(entryId);
        if (entry is null)
        {
            return;
        }

        IReadOnlyList<LReflexRule> rules = LReflexRuleRead(entry.LEntryLanguage);
        string epithet = LReflexClerkEpithet.LReflexEpithetCheck(rules)
            ? LReflexClerkEpithet.LReflexEpithetFormat(rules, _lReflexClerkReflexes.LReflexRead(entryId))
            : string.Empty;
        _lReflexClerkEntries.LEntryEpithetSave(entryId, epithet);
    }
}
