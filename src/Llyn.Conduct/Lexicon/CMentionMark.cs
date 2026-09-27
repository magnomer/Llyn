namespace Llyn.Conduct;

public sealed record CMentionMark(
    long CMentionMarkId,
    int CMentionMarkOffset,
    int CMentionMarkLength,
    long CMentionMarkEntry,
    long CMentionMarkSense);
