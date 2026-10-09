using System;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public sealed record LInflectionKind(string LInflectionKindName, string LInflectionKindPattern)
    : IEquatable<LInflectionKind>
{
    private readonly Regex _lInflectionKindRegex =
        new(LInflectionKindPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    public bool LInflectionKindCheck(string headword)
    {
        ArgumentNullException.ThrowIfNull(headword);

        try
        {
            return _lInflectionKindRegex.IsMatch(headword);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public string LInflectionKindResolve(string headword, string template)
    {
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(template);

        try
        {
            return _lInflectionKindRegex.Replace(headword, template, 1);
        }
        catch (RegexMatchTimeoutException)
        {
            return headword;
        }
    }

    public bool Equals(LInflectionKind? other) =>
        other is not null
        && string.Equals(LInflectionKindName, other.LInflectionKindName, StringComparison.Ordinal)
        && string.Equals(LInflectionKindPattern, other.LInflectionKindPattern, StringComparison.Ordinal);

    public override int GetHashCode() =>
        HashCode.Combine(LInflectionKindName, LInflectionKindPattern);
}
