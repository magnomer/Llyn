namespace Llyn.Core;

public sealed record LRequestImageAddition(
    long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestImagePick(
    long LRequestDraftId, long LRequestCardId, long LRequestImageId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestImageRemoval(long LRequestDraftId, long LRequestCardId, long LRequestImageId)
    : LRequest(LRequestDraftId);

public sealed record LRequestImageShift(
    long LRequestDraftId, long LRequestCardId, long LRequestImageId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestImageLocation(long LRequestDraftId, long LRequestImageId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);
