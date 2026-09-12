using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LExampleDraft(
    LStateValue LExampleDraftText,
    long LExampleDraftId,
    LStateAnchor LExampleDraftReference,
    LStateValue LExampleDraftTranslation,
    string LExampleDraftLanguage = "",
    IReadOnlyList<LMentionDraft>? LExampleDraftMention = null) : IEquatable<LExampleDraft>
{
    public LStateValue LExampleDraftText { get; init; } =
        LExampleDraftText ?? LStateValue.LStateValueUnspecified;

    public LStateAnchor LExampleDraftReference { get; init; } =
        LExampleDraftReference ?? LStateAnchor.LStateAnchorUnspecified;

    public LStateValue LExampleDraftTranslation { get; init; } =
        LExampleDraftTranslation ?? LStateValue.LStateValueUnspecified;

    public string LExampleDraftLanguage { get; init; } = LExampleDraftLanguage ?? string.Empty;

    public IReadOnlyList<LMentionDraft> LExampleDraftMention { get; init; } = LExampleDraftMention ?? [];

    public bool Equals(LExampleDraft? other)
    {
        return other is not null
            && LExampleDraftId == other.LExampleDraftId
            && LExampleDraftText.Equals(other.LExampleDraftText)
            && LExampleDraftReference.Equals(other.LExampleDraftReference)
            && LExampleDraftTranslation.Equals(other.LExampleDraftTranslation)
            && LExampleDraftLanguage == other.LExampleDraftLanguage
            && LExampleDraftMention.SequenceEqual(other.LExampleDraftMention);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LExampleDraftId,
            LExampleDraftText,
            LExampleDraftReference,
            LExampleDraftTranslation,
            LExampleDraftLanguage,
            LExampleDraftMention.Count);
    }

    public LExampleDraft LExampleDraftNormalize()
    {
        return this with
        {
            LExampleDraftText = LExampleDraftText.LStateValueNormalize(),
            LExampleDraftReference = LExampleDraftReference.LStateAnchorNormalize(),
            LExampleDraftTranslation = LExampleDraftTranslation.LStateValueNormalize(),
            LExampleDraftMention = LMentionDraft.LMentionDraftSort(LExampleDraftMention),
        };
    }

    public static LExampleDraft LExampleDraftCreate(string text)
    {
        return new LExampleDraft(
            LStateValue.LStateValueRead(text),
            0,
            LStateAnchor.LStateAnchorUnspecified,
            LStateValue.LStateValueUnspecified);
    }
}
