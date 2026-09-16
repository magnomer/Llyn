using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public sealed record LAnatomyPattern(
    string LAnatomyPatternRegex,
    IReadOnlyList<LRespellingRule>? LAnatomyPatternRewrites = null,
    bool LAnatomyPatternDecomposed = false) : IEquatable<LAnatomyPattern>
{
    public const string LAnatomyPatternOnset = "onset";

    public const string LAnatomyPatternVowel = "vowel";

    public const string LAnatomyPatternCoda = "coda";

    public const string LAnatomyPatternTone = "tone";

    private readonly Regex _lAnatomyPatternCompiled = new(LAnatomyPatternRegex, RegexOptions.CultureInvariant);

    public IReadOnlyList<LRespellingRule> LAnatomyPatternRewrites { get; init; } = LAnatomyPatternRewrites ?? [];

    public LAnatomyPiece LAnatomyPatternResolve(string reading)
    {
        ArgumentNullException.ThrowIfNull(reading);

        string prepared = LAnatomyPatternPrepare(reading.Trim());
        if (prepared.Length == 0)
        {
            return LAnatomyPiece.LAnatomyPieceEmpty;
        }

        Match match = _lAnatomyPatternCompiled.Match(prepared);
        if (!match.Success)
        {
            return LAnatomyPiece.LAnatomyPieceEmpty;
        }

        return new LAnatomyPiece(
            match.Groups[LAnatomyPatternOnset].Value,
            match.Groups[LAnatomyPatternVowel].Value,
            match.Groups[LAnatomyPatternCoda].Value,
            match.Groups[LAnatomyPatternTone].Value);
    }

    public bool Equals(LAnatomyPattern? other) =>
        other is not null
        && string.Equals(LAnatomyPatternRegex, other.LAnatomyPatternRegex, StringComparison.Ordinal)
        && LAnatomyPatternDecomposed == other.LAnatomyPatternDecomposed
        && LAnatomyPatternRewrites.SequenceEqual(other.LAnatomyPatternRewrites);

    public override int GetHashCode() =>
        HashCode.Combine(LAnatomyPatternRegex, LAnatomyPatternDecomposed, LAnatomyPatternRewrites.Count);

    private string LAnatomyPatternPrepare(string reading)
    {
        string result = LAnatomyPatternDecomposed
            ? reading.Normalize(NormalizationForm.FormD)
            : reading.Normalize(NormalizationForm.FormC);
        foreach (LRespellingRule rule in LAnatomyPatternRewrites)
        {
            result = rule.LRespellingRuleResolve(result);
        }

        return result.Trim();
    }
}
