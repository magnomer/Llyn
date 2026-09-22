using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEtymology(
    long LEtymologyId,
    long LEtymologyEntryId,
    string LEtymologyText,
    IReadOnlyList<LMention>? LEtymologyMentions = null)
{
    public string LEtymologyText { get; init; } = LEtymologyText ?? string.Empty;

    public IReadOnlyList<LMention> LEtymologyMentions { get; init; } =
        LMention.LMentionSort(LEtymologyMentions ?? []);

    public bool LEtymologyNarrated => LEtymologyText.Trim().Length > 0;
}
