using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDiweiClerk
{
    private readonly LDiweiVault _lDiweiClerkDiwei;
    private readonly LEntryVault _lDiweiClerkEntries;
    private readonly LReflexVault _lDiweiClerkReflexes;
    private readonly LLanguageCache _lDiweiClerkLanguages;

    public LDiweiClerk(LRig rig, LLanguageCache languages)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        _lDiweiClerkDiwei = rig.LRigDiwei;
        _lDiweiClerkEntries = rig.LRigEntries;
        _lDiweiClerkReflexes = rig.LRigReflexes;
        _lDiweiClerkLanguages = languages;
    }

    public IReadOnlyList<LDiwei> LDiweiClerkRead(string language, string kind)
    {
        return _lDiweiClerkDiwei.LDiweiRead(language, kind);
    }

    public LDiwei? LDiweiClerkRead(long? id)
    {
        return id is long wanted ? _lDiweiClerkDiwei.LDiweiRead(wanted) : null;
    }

    public LDiwei? LDiweiClerkFind(string language, string kind, string key)
    {
        return _lDiweiClerkDiwei.LDiweiFind(language, kind, key);
    }

    public IReadOnlyList<LFanqieRow> LDiweiFanqieRead(LDiwei diwei)
    {
        ArgumentNullException.ThrowIfNull(diwei);
        return _lDiweiClerkDiwei.LDiweiFanqieRead(diwei.LDiweiId);
    }

    public IReadOnlyList<LEntry> LDiweiEntryScan(string language, IReadOnlyList<long> diweiIds, string query)
    {
        ArgumentNullException.ThrowIfNull(diweiIds);
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyList<long> ids = _lDiweiClerkDiwei.LDiweiEntryScan(language, diweiIds);
        return ids.Count == 0 ? [] : _lDiweiClerkEntries.LEntryScan(ids, query);
    }

    public LDiweiPage LDiweiPageRead(LDiwei diwei, bool switched, bool tallied, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(diwei);
        ArgumentNullException.ThrowIfNull(localize);

        return new LDiweiPage(
            diwei.LDiweiLanguage,
            diwei.LDiweiKey,
            LDiweiSection.LDiweiSectionScan(
                diwei.LDiweiKind,
                LDiweiFanqieRead(diwei),
                LHypothesisRead(diwei.LDiweiLanguage),
                LTallyRead(diwei),
                switched,
                switched && tallied,
                localize));
    }

    public IReadOnlyList<LTally> LTallyRead(LDiwei diwei)
    {
        ArgumentNullException.ThrowIfNull(diwei);

        IReadOnlyList<LFanqieRow> rows = _lDiweiClerkDiwei.LDiweiFanqieRead(diwei.LDiweiId);
        IReadOnlyDictionary<long, IReadOnlyList<LReflex>> anchored =
            _lDiweiClerkReflexes.LReflexAnchorScan(diwei.LDiweiId);
        LHypothesis? hypothesis = LHypothesisRead(diwei.LDiweiLanguage);
        Dictionary<string, List<string>> sections = new(StringComparer.Ordinal);
        List<string> order = [];
        Dictionary<string, Dictionary<string, IReadOnlyList<LReflex>>> readings = new(StringComparer.Ordinal);
        foreach (LFanqieRow row in rows)
        {
            string heading = LTallyHeadingRead(diwei.LDiweiKind, row, hypothesis);
            if (!sections.TryGetValue(heading, out List<string>? characters))
            {
                characters = [];
                sections[heading] = characters;
                readings[heading] = new Dictionary<string, IReadOnlyList<LReflex>>(StringComparer.Ordinal);
                order.Add(heading);
            }

            if (row.LFanqieRowCharacter.Length > 0 && !characters.Contains(row.LFanqieRowCharacter))
            {
                characters.Add(row.LFanqieRowCharacter);
            }

            LTallyLoad(readings[heading], row, anchored);
        }

        List<string> ranking = [];
        foreach (LReflexRule rule in LReflexRuleRead(diwei.LDiweiLanguage))
        {
            if (!ranking.Contains(rule.LReflexRuleLanguage, StringComparer.OrdinalIgnoreCase))
            {
                ranking.Add(rule.LReflexRuleLanguage);
            }
        }

        List<LTally> tallies = new(order.Count);
        foreach (string heading in order)
        {
            tallies.Add(new LTally(
                heading,
                LTallyLine.LTallyLineScan(diwei.LDiweiKind, sections[heading], readings[heading], ranking)));
        }

        return tallies;
    }

    private LHypothesis? LHypothesisRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? null
            : _lDiweiClerkLanguages.LLanguageCacheRead(language).LLanguageHypothesis;
    }

    private IReadOnlyList<LReflexRule> LReflexRuleRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lDiweiClerkLanguages.LLanguageCacheRead(language).LLanguageReflexRules;
    }

    private static string LTallyHeadingRead(string kind, LFanqieRow row, LHypothesis? hypothesis)
    {
        if (kind != LDiwei.LDiweiRime)
        {
            return row.LFanqieRowDivision;
        }

        return hypothesis?.LHypothesisPlaceFind(row.LFanqieRowInitial)?.LHypothesisPlaceName ?? string.Empty;
    }

    private static void LTallyLoad(
        Dictionary<string, IReadOnlyList<LReflex>> readings,
        LFanqieRow row,
        IReadOnlyDictionary<long, IReadOnlyList<LReflex>> anchored)
    {
        if (row.LFanqieRowCharacter.Length == 0
            || !anchored.TryGetValue(row.LFanqieRowId, out IReadOnlyList<LReflex>? reflexes))
        {
            return;
        }

        List<LReflex> held = readings.TryGetValue(row.LFanqieRowCharacter, out IReadOnlyList<LReflex>? kept)
            ? [.. kept]
            : [];
        foreach (LReflex reflex in reflexes)
        {
            if (!held.Any(found => found.LReflexId == reflex.LReflexId))
            {
                held.Add(reflex);
            }
        }

        readings[row.LFanqieRowCharacter] = held;
    }
}
