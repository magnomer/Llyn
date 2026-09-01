namespace Llyn.Core;

public sealed record LTombstone(
    string LTombstoneEntryId,
    string LTombstoneRevisionId,
    string LTombstoneDeletedUtc);
