namespace Llyn.Core;

/// <summary>
/// One Meaning owned by an entry: a node in the entry's self-referential Meaning tree, carrying a
/// single inline definition field. <see cref="LSenseId"/> is the identity — an opaque, program-generated
/// stable id — and is the base every later relation, tag, situation, and example association targets.
/// A Meaning nests under another through <see cref="LSenseParentId"/>, which always names a Meaning in
/// the same entry; a root Meaning has no parent. There is no separate definition entity: each Meaning
/// holds exactly one definition (with its own optional language), empty when unset.
/// </summary>
/// <param name="LSenseId">Opaque, program-generated stable id.</param>
/// <param name="LSenseEntryId">Owning entry id.</param>
/// <param name="LSenseParentId">Parent Meaning id in the same entry, or <c>null</c> for a root Meaning.</param>
/// <param name="LSensePosition">Order within its siblings under the same parent.</param>
/// <param name="LSenseGloss">Optional short gloss.</param>
/// <param name="LSenseDefinitionLanguage">Optional language the definition is written in.</param>
/// <param name="LSenseDefinition">Single inline definition text; empty when unset.</param>
/// <param name="LSenseLabels">Labels as JSON array text (for example <c>["figurative"]</c>).</param>
public sealed record LSense(
    string LSenseId,
    string LSenseEntryId,
    string? LSenseParentId,
    int LSensePosition,
    string? LSenseGloss,
    string? LSenseDefinitionLanguage,
    string? LSenseDefinition,
    string LSenseLabels);
