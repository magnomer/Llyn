namespace Llyn.Core;

public sealed record LRelation(
    long LRelationId,
    long LRelationMeaningId,
    int LRelationPosition,
    string LRelationType,
    string? LRelationLabel,
    string? LRelationLabels,
    long? LRelationTargetEntry,
    long? LRelationTargetMeaning);
