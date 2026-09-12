namespace Llyn.Core;

public sealed record LRequestVideoAddition(
    long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestVideoPick(
    long LRequestDraftId, long LRequestCardId, long LRequestVideoId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestVideoRemoval(long LRequestDraftId, long LRequestCardId, long LRequestVideoId)
    : LRequest(LRequestDraftId);

public sealed record LRequestVideoShift(
    long LRequestDraftId, long LRequestCardId, long LRequestVideoId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestVideoLocation(long LRequestDraftId, long LRequestVideoId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestVideoSpan(long LRequestDraftId, long LRequestVideoId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);
