using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestCardAddition(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestParentId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestCardRemoval(long LRequestDraftId, long LRequestCardId)
    : LRequest(LRequestDraftId);

public sealed record LRequestCardShift(
    long LRequestDraftId, long LRequestCardId, long LRequestParentId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestCardTitle(long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestCardTitle)}:{LRequestCardId}";
}

public sealed record LRequestCardExpression(long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestCardExpression)}:{LRequestCardId}";
}

public sealed record LRequestCardMeaning(long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestCardMeaning)}:{LRequestCardId}";
}
