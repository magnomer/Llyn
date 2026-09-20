using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public sealed record LHypothesisTone(
    string LHypothesisToneOnset,
    IReadOnlyList<LRespellingRule> LHypothesisToneRules,
    string LHypothesisToneClass)
    : IEquatable<LHypothesisTone>
{
    private readonly Regex? _lHypothesisToneRegex = LHypothesisToneOnset.Length == 0
        ? null
        : new Regex(LHypothesisToneOnset, RegexOptions.CultureInvariant);

    public bool LHypothesisToneMatch(string onset)
    {
        ArgumentNullException.ThrowIfNull(onset);

        return _lHypothesisToneRegex is null || _lHypothesisToneRegex.IsMatch(onset);
    }

    public string LHypothesisToneResolve(string syllable)
    {
        ArgumentNullException.ThrowIfNull(syllable);

        string text = syllable;
        foreach (LRespellingRule rule in LHypothesisToneRules)
        {
            text = rule.LRespellingRuleResolve(text);
        }

        return text;
    }

    public bool Equals(LHypothesisTone? other) =>
        other is not null
        && string.Equals(LHypothesisToneOnset, other.LHypothesisToneOnset, StringComparison.Ordinal)
        && string.Equals(LHypothesisToneClass, other.LHypothesisToneClass, StringComparison.Ordinal)
        && LHypothesisToneRules.SequenceEqual(other.LHypothesisToneRules);

    public override int GetHashCode() =>
        HashCode.Combine(LHypothesisToneOnset, LHypothesisToneClass, LHypothesisToneRules.Count);
}
