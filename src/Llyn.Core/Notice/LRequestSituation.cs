namespace Llyn.Core;

public sealed record LRequestSituationAddition(
    long LRequestDraftId, long LRequestCardId, LStateValue LRequestValue, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationPick(
    long LRequestDraftId, long LRequestCardId, long LRequestSituationId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSituationId)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationShift(
    long LRequestDraftId, long LRequestCardId, long LRequestSituationId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationTitle(long LRequestDraftId, long LRequestSituationId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationDescription(
    long LRequestDraftId, long LRequestSituationId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationKind(long LRequestDraftId, long LRequestSituationId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);
