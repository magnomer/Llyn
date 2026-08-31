namespace Llyn.Core;

/// <summary>
/// One lexical relation originating from a Meaning and pointing at exactly one target — an Entry or
/// another Meaning — through a checked reference, never a free-text id. <see cref="LRelationId"/> is
/// the identity; <see cref="LRelationSenseId"/> names the origin Meaning the relation hangs from.
/// The target is discriminated: exactly one of <see cref="LRelationTargetEntry"/> and
/// <see cref="LRelationTargetSense"/> is set, and the store enforces that XOR. This is also the
/// synonym/collocation interlink mechanism later jobs reuse, so the target model stays reusable.
/// </summary>
/// <param name="LRelationId">Opaque, program-generated stable id.</param>
/// <param name="LRelationSenseId">Origin Meaning id the relation hangs from.</param>
/// <param name="LRelationPosition">Order among the origin Meaning's relations.</param>
/// <param name="LRelationType">Stable relation-type id (for example the synonym or antonym type).</param>
/// <param name="LRelationLabel">Optional single label text.</param>
/// <param name="LRelationLabels">Labels as JSON array text (same format as job04), or <c>null</c>.</param>
/// <param name="LRelationTargetEntry">Target Entry id when the relation points at an Entry, else <c>null</c>.</param>
/// <param name="LRelationTargetSense">Target Meaning id when the relation points at a Meaning, else <c>null</c>.</param>
public sealed record LRelation(
    string LRelationId,
    string LRelationSenseId,
    int LRelationPosition,
    string LRelationType,
    string? LRelationLabel,
    string? LRelationLabels,
    string? LRelationTargetEntry,
    string? LRelationTargetSense);
