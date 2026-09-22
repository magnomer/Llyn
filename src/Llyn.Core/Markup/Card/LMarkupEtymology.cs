using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMarkupEtymology(
    string LMarkupEtymologyText = "",
    IReadOnlyList<LMarkupMention>? LMarkupEtymologyMention = null) : IEquatable<LMarkupEtymology>
{
    public string LMarkupEtymologyText { get; init; } = LMarkupEtymologyText ?? string.Empty;

    public IReadOnlyList<LMarkupMention> LMarkupEtymologyMention { get; init; } = LMarkupEtymologyMention ?? [];

    public bool Equals(LMarkupEtymology? other)
    {
        return other is not null
            && LMarkupEtymologyText == other.LMarkupEtymologyText
            && LMarkupEtymologyMention.SequenceEqual(other.LMarkupEtymologyMention);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LMarkupEtymologyText, LMarkupEtymologyMention.Count);
    }
}
