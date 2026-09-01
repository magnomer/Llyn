namespace Llyn.Core;

/// <summary>
/// One synonym interlink hanging from a Collocation. A collocation synonym is never Entry-owned text: it
/// points at a lexical target through the same discriminated, checked target model job05 defined for
/// <see cref="LRelation"/> — exactly one of <see cref="LSynonymTargetEntry"/> and
/// <see cref="LSynonymTargetSense"/> is set, and both the store and the table's check constraint enforce
/// that XOR.
/// <para>
/// TODO: the precise targeting rules for a collocation synonym are not finalized — which target kinds are
/// legal, whether a synonym may point at another Collocation, and whether the link is symmetric are still
/// open. This type is the storage seam only; tighten it once the rules are decided.
/// </para>
/// </summary>
/// <param name="LSynonymId">Opaque, program-generated stable id.</param>
/// <param name="LSynonymCollocationId">Origin Collocation id the synonym hangs from.</param>
/// <param name="LSynonymPosition">Order among the origin Collocation's synonyms.</param>
/// <param name="LSynonymTargetEntry">Target Entry id when the synonym points at an Entry, else <c>null</c>.</param>
/// <param name="LSynonymTargetSense">Target Meaning id when the synonym points at a Meaning, else <c>null</c>.</param>
public sealed record LSynonym(
    string LSynonymId,
    string LSynonymCollocationId,
    int LSynonymPosition,
    string? LSynonymTargetEntry,
    string? LSynonymTargetSense);
