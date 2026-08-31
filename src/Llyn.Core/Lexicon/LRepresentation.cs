namespace Llyn.Core;

/// <summary>
/// One notational representation of a pronunciation, ordered within it. Identity is
/// <c>(pronunciation_id, position)</c>: the representation is subordinate to its
/// <see cref="LRepresentationPronunciationId"/> parent, and reordering changes
/// <see cref="LRepresentationPosition"/> only. It records a value in a named transcription
/// <see cref="LRepresentationSystem"/> playing a given <see cref="LRepresentationRole"/>.
/// </summary>
/// <param name="LRepresentationPronunciationId">Parent pronunciation id.</param>
/// <param name="LRepresentationPosition">Order within the parent pronunciation.</param>
/// <param name="LRepresentationSystem">The transcription system the text is written in.</param>
/// <param name="LRepresentationRole">The role this representation plays for the pronunciation.</param>
/// <param name="LRepresentationText">The representation text.</param>
/// <param name="LRepresentationLocalTone">Optional local tone notation; NULL when absent.</param>
public sealed record LRepresentation(
    string LRepresentationPronunciationId,
    int LRepresentationPosition,
    string LRepresentationSystem,
    string LRepresentationRole,
    string LRepresentationText,
    string? LRepresentationLocalTone);
