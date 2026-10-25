using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestImageAddition(
    long LRequestDraftId,
    LCardKind LRequestKind,
    long LRequestCardId,
    LStateWritten LRequestValue,
    int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestImageAddition);
}

public sealed record LRequestImagePick(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestImageId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestImagePick);
}

public sealed record LRequestImageRemoval(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestImageId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestImageRemoval);
}

public sealed record LRequestImageShift(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestImageId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestImageShift);
}

public sealed record LRequestImageLocation(long LRequestDraftId, long LRequestImageId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestImageLocation)}:{LRequestImageId}";
}
