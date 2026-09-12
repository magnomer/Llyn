using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LExample(
    long LExampleId,
    string LExampleLanguage,
    LStateValue LExampleText,
    LStateValue LExampleTranslation,
    LStateAnchor LExampleSource,
    IReadOnlyList<LMention>? LExampleMention = null) : IEquatable<LExample>
{
    public LStateValue LExampleText { get; init; } = LExampleText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleTranslation { get; init; } =
        LExampleTranslation ?? LStateValue.LStateValueUnspecified;

    public LStateAnchor LExampleSource { get; init; } = LExampleSource ?? LStateAnchor.LStateAnchorUnspecified;

    public IReadOnlyList<LMention> LExampleMention { get; init; } = LExampleMention ?? [];

    public bool Equals(LExample? other)
    {
        return other is not null
            && LExampleId == other.LExampleId
            && LExampleLanguage == other.LExampleLanguage
            && LExampleText.Equals(other.LExampleText)
            && LExampleTranslation.Equals(other.LExampleTranslation)
            && LExampleSource.Equals(other.LExampleSource)
            && LExampleMention.SequenceEqual(other.LExampleMention);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LExampleId, LExampleLanguage, LExampleText, LExampleTranslation, LExampleSource, LExampleMention.Count);
    }

    public LExample LExampleNormalize()
    {
        return this with
        {
            LExampleText = LExampleText.LStateValueNormalize(),
            LExampleTranslation = LExampleTranslation.LStateValueNormalize(),
            LExampleSource = LExampleSource.LStateAnchorNormalize(),
            LExampleMention = LMention.LMentionSort(LExampleMention),
        };
    }
}
