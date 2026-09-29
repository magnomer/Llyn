namespace Llyn.Conduct;

public sealed record CMentionDraft(
    long CMentionDraftId,
    long CMentionDraftEntry,
    int CMentionDraftOffset,
    int CMentionDraftLength,
    long CMentionDraftSense);
