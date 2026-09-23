using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LExampleDraft(
    LStateValue LExampleDraftText,
    long LExampleDraftId,
    LStateAnchor LExampleDraftReference,
    string LExampleDraftLanguage = "",
    IReadOnlyList<LGlossDraft>? LExampleDraftGloss = null,
    IReadOnlyList<LMentionDraft>? LExampleDraftMention = null) : IEquatable<LExampleDraft>
{
    public LStateValue LExampleDraftText { get; init; } =
        LExampleDraftText ?? LStateValue.LStateValueUnspecified;

    public LStateAnchor LExampleDraftReference { get; init; } =
        LExampleDraftReference ?? LStateAnchor.LStateAnchorUnspecified;

    public string LExampleDraftLanguage { get; init; } = LExampleDraftLanguage ?? string.Empty;

    public IReadOnlyList<LGlossDraft> LExampleDraftGloss { get; init; } = LExampleDraftGloss ?? [];

    public IReadOnlyList<LMentionDraft> LExampleDraftMention { get; init; } = LExampleDraftMention ?? [];

    public bool Equals(LExampleDraft? other)
    {
        return other is not null
            && LExampleDraftId == other.LExampleDraftId
            && LExampleDraftText.Equals(other.LExampleDraftText)
            && LExampleDraftReference.Equals(other.LExampleDraftReference)
            && LExampleDraftLanguage == other.LExampleDraftLanguage
            && LExampleDraftGloss.SequenceEqual(other.LExampleDraftGloss)
            && LExampleDraftMention.SequenceEqual(other.LExampleDraftMention);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LExampleDraftId,
            LExampleDraftText,
            LExampleDraftReference,
            LExampleDraftLanguage,
            LExampleDraftGloss.Count,
            LExampleDraftMention.Count);
    }

    public LExampleDraft LExampleDraftNormalize()
    {
        return this with
        {
            LExampleDraftText = LExampleDraftText.LStateValueNormalize(),
            LExampleDraftReference = LExampleDraftReference.LStateAnchorNormalize(),
            LExampleDraftGloss = LGlossDraft.LGlossDraftNormalize(LExampleDraftGloss),
            LExampleDraftMention = LMentionDraft.LMentionDraftSort(LExampleDraftMention),
        };
    }

    public LMentionDraft? LExampleDraftFind(LMentionDraft span)
    {
        return LMentionSpan.LMentionSpanFind(LExampleDraftMention, span);
    }

    public static LExampleDraft LExampleDraftCreate(string text)
    {
        return new LExampleDraft(
            LStateValue.LStateValueRead(text),
            0,
            LStateAnchor.LStateAnchorUnspecified);
    }

    public static LExampleDraft LExampleDraftCreate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);

        List<LGlossDraft> glosses = new(example.LExampleGloss.Count);
        foreach (LGloss gloss in example.LExampleGloss)
        {
            glosses.Add(LGlossDraft.LGlossDraftCreate(gloss));
        }

        List<LMentionDraft> mentions = new(example.LExampleMention.Count);
        foreach (LMention mention in example.LExampleMention)
        {
            mentions.Add(LMentionDraft.LMentionDraftCreate(mention));
        }

        return new LExampleDraft(
            example.LExampleText,
            example.LExampleId,
            example.LExampleSource,
            example.LExampleLanguage,
            glosses,
            mentions);
    }
}
