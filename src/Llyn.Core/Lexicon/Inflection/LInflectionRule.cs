using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public sealed record LInflectionRule(
    string LInflectionRulePattern,
    string LInflectionRuleReplacement,
    string? LInflectionRuleKind)
    : IEquatable<LInflectionRule>
{
    private readonly Regex _lInflectionRuleRegex =
        new(LInflectionRulePattern, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

    public string LInflectionRuleResolve(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        try
        {
            return _lInflectionRuleRegex.Replace(text, LInflectionRuleReplacement);
        }
        catch (RegexMatchTimeoutException)
        {
            return text;
        }
    }

    public IReadOnlyList<Match> LInflectionRuleScan(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        try
        {
            return _lInflectionRuleRegex.Matches(text).ToList();
        }
        catch (RegexMatchTimeoutException)
        {
            return [];
        }
    }

    public bool Equals(LInflectionRule? other) =>
        other is not null
        && string.Equals(LInflectionRulePattern, other.LInflectionRulePattern, StringComparison.Ordinal)
        && string.Equals(LInflectionRuleReplacement, other.LInflectionRuleReplacement, StringComparison.Ordinal)
        && string.Equals(LInflectionRuleKind, other.LInflectionRuleKind, StringComparison.Ordinal);

    public override int GetHashCode() =>
        HashCode.Combine(LInflectionRulePattern, LInflectionRuleReplacement, LInflectionRuleKind);
}
