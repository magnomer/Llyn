using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestSituationAddition(
    long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationAddition);
}

public sealed record LRequestSituationPick(
    long LRequestDraftId, long LRequestCardId, long LRequestSituationId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationPick);
}

public sealed record LRequestSituationRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSituationId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationRemoval);
}

public sealed record LRequestSituationShift(
    long LRequestDraftId, long LRequestCardId, long LRequestSituationId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationShift);
}

public sealed record LRequestSituationTitle(long LRequestDraftId, long LRequestSituationId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationTitle);
}

public sealed record LRequestSituationDescription(
    long LRequestDraftId, long LRequestSituationId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationDescription);
}

public sealed record LRequestSituationKind(long LRequestDraftId, long LRequestSituationId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationKind);
}

public sealed record LRequestSituationBody(
    long LRequestDraftId,
    long LRequestSituationId,
    LStateWritten LRequestTitle,
    LStateWritten LRequestDescription,
    LStateWritten LRequestKind)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSituationBody);
}
