using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LExample(
    long LExampleId,
    string LExampleLanguage,
    LStateValue LExampleText,
    LStateAnchor LExampleSource,
    IReadOnlyList<LGloss>? LExampleGloss = null,
    IReadOnlyList<LMention>? LExampleMention = null) : IEquatable<LExample>
{
    public LStateValue LExampleText { get; init; } = LExampleText ?? LStateValue.LStateValueUnspecified;

    public LStateAnchor LExampleSource { get; init; } = LExampleSource ?? LStateAnchor.LStateAnchorUnspecified;

    public IReadOnlyList<LGloss> LExampleGloss { get; init; } = LExampleGloss ?? [];

    public IReadOnlyList<LMention> LExampleMention { get; init; } = LExampleMention ?? [];

    public bool Equals(LExample? other)
    {
        return other is not null
            && LExampleId == other.LExampleId
            && LExampleLanguage == other.LExampleLanguage
            && LExampleText.Equals(other.LExampleText)
            && LExampleSource.Equals(other.LExampleSource)
            && LExampleGloss.SequenceEqual(other.LExampleGloss)
            && LExampleMention.SequenceEqual(other.LExampleMention);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LExampleId, LExampleLanguage, LExampleText, LExampleSource, LExampleGloss.Count, LExampleMention.Count);
    }

    public LExample LExampleNormalize()
    {
        return this with
        {
            LExampleText = LExampleText.LStateValueNormalize(),
            LExampleSource = LExampleSource.LStateAnchorNormalize(),
            LExampleGloss = LGloss.LGlossNormalize(LExampleGloss),
            LExampleMention = LMention.LMentionSort(LExampleMention),
        };
    }
}
