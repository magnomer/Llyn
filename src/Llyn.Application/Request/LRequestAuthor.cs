using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestAuthorAddition(
    long LRequestDraftId,
    string LRequestText,
    int LRequestPosition,
    long LRequestFormerId = 0)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAuthorAddition);
}

public sealed record LRequestAuthorPick(
    long LRequestDraftId,
    long LRequestAuthorId,
    int LRequestPosition,
    long LRequestFormerId = 0)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAuthorPick);
}

public sealed record LRequestAuthorRemoval(long LRequestDraftId, long LRequestAuthorId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAuthorRemoval);
}

public sealed record LRequestAuthorShift(long LRequestDraftId, long LRequestAuthorId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAuthorShift);
}

public sealed record LRequestAuthorState(long LRequestDraftId, LState LRequestState)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAuthorState);
}

public sealed record LRequestAuthorName(long LRequestDraftId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAuthorName);
}
