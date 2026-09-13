using System;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public sealed record LParadigmRule(string LParadigmRulePattern, string LParadigmRuleReplacement)
    : IEquatable<LParadigmRule>
{
    private static readonly TimeSpan LParadigmRulePatience = TimeSpan.FromSeconds(1);

    private readonly Regex _lParadigmRuleRegex =
        new(LParadigmRulePattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, LParadigmRulePatience);

    public string? LParadigmRuleResolve(string headword)
    {
        ArgumentNullException.ThrowIfNull(headword);

        try
        {
            return _lParadigmRuleRegex.IsMatch(headword)
                ? _lParadigmRuleRegex.Replace(headword, LParadigmRuleReplacement, 1)
                : null;
        }
        catch (RegexMatchTimeoutException)
        {
            return null;
        }
    }

    public bool Equals(LParadigmRule? other) =>
        other is not null
        && string.Equals(LParadigmRulePattern, other.LParadigmRulePattern, StringComparison.Ordinal)
        && string.Equals(LParadigmRuleReplacement, other.LParadigmRuleReplacement, StringComparison.Ordinal);

    public override int GetHashCode() =>
        HashCode.Combine(LParadigmRulePattern, LParadigmRuleReplacement);
}
