using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestCardAddition(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestParentId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestCardAddition);
}

public sealed record LRequestCardRemoval(long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestCardRemoval);
}

public sealed record LRequestCardShift(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestParentId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestCardShift);
}

public sealed record LRequestCardTitle(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestCardTitle)}:{LRequestKind}:{LRequestCardId}";
}

public sealed record LRequestCardExpression(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestCardExpression)}:{LRequestKind}:{LRequestCardId}";
}

public sealed record LRequestCardMeaning(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestCardMeaning)}:{LRequestKind}:{LRequestCardId}";
}

public sealed record LRequestSenseAddition(long LRequestDraftId, long LRequestCardId, long LRequestSenseId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestSenseAddition)}:{LRequestCardId}:{LRequestSenseId}";
}

public sealed record LRequestSenseRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSenseId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestSenseRemoval)}:{LRequestCardId}:{LRequestSenseId}";
}
