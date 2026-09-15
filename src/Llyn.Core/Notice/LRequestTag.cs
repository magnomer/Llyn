namespace Llyn.Core;

public sealed record LRequestTagAddition(
    long LRequestDraftId, long LRequestCardId, string LRequestText, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestTagPick(
    long LRequestDraftId, long LRequestCardId, long LRequestTagId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestTagRemoval(long LRequestDraftId, long LRequestCardId, long LRequestTagId)
    : LRequest(LRequestDraftId);

public sealed record LRequestTagShift(
    long LRequestDraftId, long LRequestCardId, long LRequestTagId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestTagText(long LRequestDraftId, long LRequestTagId, string LRequestText)
    : LRequest(LRequestDraftId);
