namespace Llyn.Core;

public sealed record LRepresentation(
    string LRepresentationPronunciationId,
    int LRepresentationPosition,
    string LRepresentationSystem,
    string LRepresentationRole,
    string LRepresentationText,
    string? LRepresentationLocalTone);
