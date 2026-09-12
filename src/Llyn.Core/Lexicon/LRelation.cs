namespace Llyn.Core;

public sealed record LRelation(
    long LRelationId,
    long LRelationMeaningId,
    int LRelationPosition,
    string LRelationType,
    string? LRelationLabel,
    string? LRelationLabels,
    LStateAnchor LRelationTargetEntry,
    LStateAnchor LRelationTargetMeaning)
{
    public LStateAnchor LRelationTargetEntry { get; init; } =
        LRelationTargetEntry ?? LStateAnchor.LStateAnchorUnspecified;

    public LStateAnchor LRelationTargetMeaning { get; init; } =
        LRelationTargetMeaning ?? LStateAnchor.LStateAnchorUnspecified;
}
