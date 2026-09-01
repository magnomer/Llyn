namespace Llyn.Core;

/// <summary>
/// The record that an Entry was deleted: which Entry, under which revision, and when. A tombstone
/// outlives the Entry it names — <see cref="LTombstoneEntryId"/> is recorded text, not a reference to
/// a row that still exists — so the history stays readable after the Entry is gone. One deleted Entry
/// leaves exactly one tombstone.
/// </summary>
/// <param name="LTombstoneEntryId">Id of the deleted Entry; recorded text, not a reference.</param>
/// <param name="LTombstoneRevisionId">The <see cref="LRevision"/> the deletion was recorded under.</param>
/// <param name="LTombstoneDeletedUtc">Round-trip UTC timestamp of the deletion.</param>
public sealed record LTombstone(
    string LTombstoneEntryId,
    string LTombstoneRevisionId,
    string LTombstoneDeletedUtc);
