namespace Llyn.Core;

public sealed record LRequestRegisterAddition(
    long LRequestDraftId, long LRequestCardId, LStateValue LRequestValue, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestRegisterPick(
    long LRequestDraftId, long LRequestCardId, long LRequestRegisterId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestRegisterRemoval(long LRequestDraftId, long LRequestCardId, long LRequestRegisterId)
    : LRequest(LRequestDraftId);

public sealed record LRequestRegisterShift(
    long LRequestDraftId, long LRequestCardId, long LRequestRegisterId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestRegisterName(long LRequestDraftId, long LRequestRegisterId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);
