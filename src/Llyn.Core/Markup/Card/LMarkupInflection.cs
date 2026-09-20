using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMarkupInflection(
    string LMarkupInflectionText,
    string? LMarkupInflectionLocal,
    string LMarkupInflectionSpeech = "",
    IReadOnlyList<string>? LMarkupInflectionMorphology = null) : IEquatable<LMarkupInflection>
{
    public string LMarkupInflectionText { get; init; } = LMarkupInflectionText ?? string.Empty;

    public string LMarkupInflectionSpeech { get; init; } = LMarkupInflectionSpeech ?? string.Empty;

    public IReadOnlyList<string> LMarkupInflectionMorphology { get; init; } = LMarkupInflectionMorphology ?? [];

    public bool Equals(LMarkupInflection? other)
    {
        return other is not null
            && LMarkupInflectionText == other.LMarkupInflectionText
            && LMarkupInflectionLocal == other.LMarkupInflectionLocal
            && LMarkupInflectionSpeech == other.LMarkupInflectionSpeech
            && LMarkupInflectionMorphology.SequenceEqual(other.LMarkupInflectionMorphology);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LMarkupInflectionText,
            LMarkupInflectionLocal,
            LMarkupInflectionSpeech,
            LMarkupInflectionMorphology.Count);
    }
}
