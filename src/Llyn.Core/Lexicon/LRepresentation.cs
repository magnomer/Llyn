namespace Llyn.Core;

public sealed record LRepresentation(
    long LRepresentationPronunciationId,
    int LRepresentationPosition,
    string LRepresentationSystem,
    string LRepresentationRole,
    string LRepresentationText,
    string? LRepresentationLocalTone);
