using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEtymology(
    long LEtymologyId,
    string LEtymologyText,
    IReadOnlyList<LMention>? LEtymologyMentions = null)
{
    public string LEtymologyText { get; init; } = LEtymologyText ?? string.Empty;

    public IReadOnlyList<LMention> LEtymologyMentions { get; init; } =
        LMention.LMentionSort(LEtymologyMentions ?? []);
}
