using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMarkupExample(
    LStateValue LMarkupExampleText,
    string LMarkupExampleLanguage = "",
    IReadOnlyList<LGlossDraft>? LMarkupExampleGloss = null,
    IReadOnlyList<LMarkupMention>? LMarkupExampleMention = null,
    LMarkupReference? LMarkupExampleReference = null) : IEquatable<LMarkupExample>
{
    public LStateValue LMarkupExampleText { get; init; } = LMarkupExampleText ?? LStateValue.LStateValueUnspecified;

    public string LMarkupExampleLanguage { get; init; } = LMarkupExampleLanguage ?? string.Empty;

    public IReadOnlyList<LGlossDraft> LMarkupExampleGloss { get; init; } = LMarkupExampleGloss ?? [];

    public IReadOnlyList<LMarkupMention> LMarkupExampleMention { get; init; } = LMarkupExampleMention ?? [];

    public bool Equals(LMarkupExample? other)
    {
        return other is not null
            && LMarkupExampleText.Equals(other.LMarkupExampleText)
            && LMarkupExampleLanguage == other.LMarkupExampleLanguage
            && LMarkupExampleGloss.SequenceEqual(other.LMarkupExampleGloss)
            && LMarkupExampleMention.SequenceEqual(other.LMarkupExampleMention)
            && Equals(LMarkupExampleReference, other.LMarkupExampleReference);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LMarkupExampleText,
            LMarkupExampleLanguage,
            LMarkupExampleGloss.Count,
            LMarkupExampleMention.Count,
            LMarkupExampleReference);
    }
}
