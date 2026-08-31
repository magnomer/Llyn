namespace Llyn.Core;

/// <summary>
/// A written form of an entry, ordered within it. Identity is <c>(entry_id, position)</c>: the form
/// is subordinate to its <see cref="LFormEntryId"/> parent, and reordering changes
/// <see cref="LFormPosition"/> only.
/// </summary>
/// <param name="LFormEntryId">Parent entry id.</param>
/// <param name="LFormPosition">Order within the parent entry.</param>
/// <param name="LFormText">The written form.</param>
/// <param name="LFormLocal">Optional local representation.</param>
/// <param name="LFormRole">Stable role identifier.</param>
public sealed record LForm(
    string LFormEntryId,
    int LFormPosition,
    string LFormText,
    string? LFormLocal,
    string LFormRole);
