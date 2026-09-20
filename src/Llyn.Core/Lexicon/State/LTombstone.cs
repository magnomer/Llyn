namespace Llyn.Core;

public sealed record LTombstone(
    long LTombstoneEntryId,
    long LTombstoneRevisionId,
    string LTombstoneDeletedUtc);
