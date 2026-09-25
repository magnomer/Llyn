using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEtymologyDraft(
    string LEtymologyDraftText = "",
    IReadOnlyList<LMentionDraft>? LEtymologyDraftMentions = null,
    IReadOnlyList<long>? LEtymologyDraftEtymons = null,
    long LEtymologyDraftId = 0)
{
    public string LEtymologyDraftText { get; init; } = LEtymologyDraftText ?? string.Empty;

    public IReadOnlyList<LMentionDraft> LEtymologyDraftMentions { get; init; } =
        LMentionDraft.LMentionDraftSort(LEtymologyDraftMentions ?? []);

    public IReadOnlyList<long> LEtymologyDraftEtymons { get; init; } = LEtymologyDraftEtymons ?? [];

    public bool LEtymologyDraftNarrated => LEtymologyDraftText.Trim().Length > 0;

    public bool LEtymologyDraftLinked => LEtymologyDraftEtymons.Count > 0;

    public bool LEtymologyDraftEmpty =>
        !LEtymologyDraftNarrated && !LEtymologyDraftLinked && LEtymologyDraftMentions.Count == 0;

    public static LEtymologyDraft LEtymologyDraftCreate(LEtymology? etymology, IReadOnlyList<long> etymons)
    {
        List<LMentionDraft> mentions = [];
        foreach (LMention mention in etymology?.LEtymologyMentions ?? [])
        {
            mentions.Add(LMentionDraft.LMentionDraftCreate(mention));
        }

        return new LEtymologyDraft(
            etymology?.LEtymologyText ?? string.Empty,
            mentions,
            etymons,
            etymology?.LEtymologyId ?? 0);
    }

    public LEtymology LEtymologyDraftResolve()
    {
        List<LMention> mentions = [];
        foreach (LMentionDraft mention in LEtymologyDraftMentions)
        {
            mentions.Add(mention.LMentionDraftResolve());
        }

        return new LEtymology(LEtymologyDraftId, LEtymologyDraftText, mentions);
    }

    public LMentionDraft? LEtymologyDraftFind(LMentionDraft span)
    {
        return LMentionSpan.LMentionSpanFind(LEtymologyDraftMentions, span);
    }
}
