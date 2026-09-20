using System;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public sealed record LRespellingRule(string LRespellingRulePattern, string LRespellingRuleReplacement)
    : IEquatable<LRespellingRule>
{
    private readonly Regex _lRespellingRuleRegex = new(LRespellingRulePattern, RegexOptions.CultureInvariant);

    public string LRespellingRuleResolve(string phonetic) =>
        _lRespellingRuleRegex.Replace(phonetic, LRespellingRuleReplacement);

    public bool Equals(LRespellingRule? other) =>
        other is not null
        && string.Equals(LRespellingRulePattern, other.LRespellingRulePattern, StringComparison.Ordinal)
        && string.Equals(LRespellingRuleReplacement, other.LRespellingRuleReplacement, StringComparison.Ordinal);

    public override int GetHashCode() =>
        HashCode.Combine(LRespellingRulePattern, LRespellingRuleReplacement);
}
