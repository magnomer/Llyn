using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMarkupCard(
    LStateValue LMarkupCardTitle,
    LStateValue LMarkupCardExpression,
    LStateValue LMarkupCardMeaning,
    IReadOnlyList<LMarkupSentence>? LMarkupCardSentence = null,
    IReadOnlyList<LSituationDraft>? LMarkupCardSituation = null,
    IReadOnlyList<LRegisterDraft>? LMarkupCardRegister = null,
    IReadOnlyList<LMarkupTranslation>? LMarkupCardTranslation = null,
    IReadOnlyList<LTagDraft>? LMarkupCardTag = null,
    IReadOnlyList<LImageDraft>? LMarkupCardImage = null,
    IReadOnlyList<LVideoDraft>? LMarkupCardVideo = null,
    IReadOnlyList<LMarkupCard>? LMarkupCardChild = null) : IEquatable<LMarkupCard>
{
    public LStateValue LMarkupCardTitle { get; init; } = LMarkupCardTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMarkupCardExpression { get; init; } =
        LMarkupCardExpression ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMarkupCardMeaning { get; init; } = LMarkupCardMeaning ?? LStateValue.LStateValueUnspecified;

    public IReadOnlyList<LMarkupSentence> LMarkupCardSentence { get; init; } = LMarkupCardSentence ?? [];

    public IReadOnlyList<LSituationDraft> LMarkupCardSituation { get; init; } = LMarkupCardSituation ?? [];

    public IReadOnlyList<LRegisterDraft> LMarkupCardRegister { get; init; } = LMarkupCardRegister ?? [];

    public IReadOnlyList<LMarkupTranslation> LMarkupCardTranslation { get; init; } = LMarkupCardTranslation ?? [];

    public IReadOnlyList<LTagDraft> LMarkupCardTag { get; init; } = LMarkupCardTag ?? [];

    public IReadOnlyList<LImageDraft> LMarkupCardImage { get; init; } = LMarkupCardImage ?? [];

    public IReadOnlyList<LVideoDraft> LMarkupCardVideo { get; init; } = LMarkupCardVideo ?? [];

    public IReadOnlyList<LMarkupCard> LMarkupCardChild { get; init; } = LMarkupCardChild ?? [];

    public bool Equals(LMarkupCard? other)
    {
        return other is not null
            && LMarkupCardTitle.Equals(other.LMarkupCardTitle)
            && LMarkupCardExpression.Equals(other.LMarkupCardExpression)
            && LMarkupCardMeaning.Equals(other.LMarkupCardMeaning)
            && LMarkupCardSentence.SequenceEqual(other.LMarkupCardSentence)
            && LMarkupCardSituation.SequenceEqual(other.LMarkupCardSituation)
            && LMarkupCardRegister.SequenceEqual(other.LMarkupCardRegister)
            && LMarkupCardTranslation.SequenceEqual(other.LMarkupCardTranslation)
            && LMarkupCardTag.SequenceEqual(other.LMarkupCardTag)
            && LMarkupCardImage.SequenceEqual(other.LMarkupCardImage)
            && LMarkupCardVideo.SequenceEqual(other.LMarkupCardVideo)
            && LMarkupCardChild.SequenceEqual(other.LMarkupCardChild);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LMarkupCardTitle,
            LMarkupCardExpression,
            LMarkupCardMeaning,
            LMarkupCardSentence.Count,
            LMarkupCardChild.Count);
    }
}
