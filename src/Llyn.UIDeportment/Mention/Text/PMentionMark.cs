namespace Llyn.UIDeportment;

internal sealed record PMentionMark(
    long PMentionMarkId,
    int PMentionMarkOffset,
    int PMentionMarkLength,
    long PMentionMarkEntry,
    long PMentionMarkSense);
