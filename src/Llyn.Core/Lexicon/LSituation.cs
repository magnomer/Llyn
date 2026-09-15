using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LSituation(
    long LSituationId,
    LStateValue LSituationTitle,
    LStateValue LSituationDescription,
    LStateValue LSituationKind,
    IReadOnlyList<LImageDraft>? LSituationImage = null,
    IReadOnlyList<LVideoDraft>? LSituationVideo = null) : IEquatable<LSituation>
{
    public LStateValue LSituationTitle { get; init; } = LSituationTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationDescription { get; init; } =
        LSituationDescription ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationKind { get; init; } = LSituationKind ?? LStateValue.LStateValueUnspecified;

    public IReadOnlyList<LImageDraft> LSituationImage { get; init; } = LSituationImage ?? [];

    public IReadOnlyList<LVideoDraft> LSituationVideo { get; init; } = LSituationVideo ?? [];

    public bool Equals(LSituation? other)
    {
        return other is not null
            && LSituationId == other.LSituationId
            && LSituationTitle.Equals(other.LSituationTitle)
            && LSituationDescription.Equals(other.LSituationDescription)
            && LSituationKind.Equals(other.LSituationKind)
            && LSituationImage.SequenceEqual(other.LSituationImage)
            && LSituationVideo.SequenceEqual(other.LSituationVideo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LSituationId, LSituationTitle, LSituationDescription, LSituationKind,
            LSituationImage.Count, LSituationVideo.Count);
    }

    public LSituation LSituationNormalize()
    {
        return this with
        {
            LSituationTitle = LSituationTitle.LStateValueNormalize(),
            LSituationDescription = LSituationDescription.LStateValueNormalize(),
            LSituationKind = LSituationKind.LStateValueNormalize(),
            LSituationImage = LSituationImage.Select(static row => row.LImageDraftNormalize()).ToList(),
            LSituationVideo = LSituationVideo.Select(static row => row.LVideoDraftNormalize()).ToList(),
        };
    }
}
