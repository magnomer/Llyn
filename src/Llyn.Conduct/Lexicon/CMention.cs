namespace Llyn.Conduct;

public sealed record CMention(
    long CMentionId,
    int CMentionOffset,
    int CMentionLength,
    long CMentionEntry,
    long CMentionSense);
