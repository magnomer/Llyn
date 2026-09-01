namespace Llyn.Core;

public sealed record LRelation(
    string LRelationId,
    string LRelationSenseId,
    int LRelationPosition,
    string LRelationType,
    string? LRelationLabel,
    string? LRelationLabels,
    string? LRelationTargetEntry,
    string? LRelationTargetSense);
