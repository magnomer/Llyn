using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestVideoAddition(
    long LRequestDraftId,
    LCardKind LRequestKind,
    long LRequestCardId,
    LStateWritten LRequestValue,
    int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestVideoAddition);
}

public sealed record LRequestVideoPick(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestVideoId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestVideoPick);
}

public sealed record LRequestVideoRemoval(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestVideoId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestVideoRemoval);
}

public sealed record LRequestVideoShift(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestVideoId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestVideoShift);
}

public sealed record LRequestVideoLocation(long LRequestDraftId, long LRequestVideoId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestVideoLocation)}:{LRequestVideoId}";
}

public sealed record LRequestVideoSpan(long LRequestDraftId, long LRequestVideoId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestVideoSpan)}:{LRequestVideoId}";
}
