using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestSituationAddition(
    long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationPick(
    long LRequestDraftId, long LRequestCardId, long LRequestSituationId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSituationId)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationShift(
    long LRequestDraftId, long LRequestCardId, long LRequestSituationId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationTitle(long LRequestDraftId, long LRequestSituationId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationDescription(
    long LRequestDraftId, long LRequestSituationId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationKind(long LRequestDraftId, long LRequestSituationId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSituationBody(
    long LRequestDraftId,
    long LRequestSituationId,
    LStateWritten LRequestTitle,
    LStateWritten LRequestDescription,
    LStateWritten LRequestKind)
    : LRequest(LRequestDraftId);
