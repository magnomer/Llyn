namespace Llyn.Core;

public sealed record LRelation(
    string LRelationId,
    string LRelationMeaningId,
    int LRelationPosition,
    string LRelationType,
    string? LRelationLabel,
    string? LRelationLabels,
    string? LRelationTargetEntry,
    string? LRelationTargetMeaning);
