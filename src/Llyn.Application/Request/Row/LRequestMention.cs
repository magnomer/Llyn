namespace Llyn.Application;

public sealed record LRequestMentionAddition(
    long LRequestDraftId,
    long LRequestCardId,
    long LRequestSentenceId,
    int LRequestOffset,
    int LRequestLength,
    long LRequestEntryId,
    long LRequestSenseId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestMentionAddition);
}

public sealed record LRequestMentionRemoval(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestMentionId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestMentionRemoval);
}

public sealed record LRequestMentionSense(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestMentionId, long LRequestSenseId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestMentionSense)}:{LRequestMentionId}";
}
