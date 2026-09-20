using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestRegisterAddition(
    long LRequestDraftId, long LRequestCardId, LStateWritten LRequestValue, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestRegisterAddition);
}

public sealed record LRequestRegisterPick(
    long LRequestDraftId, long LRequestCardId, long LRequestRegisterId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestRegisterPick);
}

public sealed record LRequestRegisterRemoval(long LRequestDraftId, long LRequestCardId, long LRequestRegisterId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestRegisterRemoval);
}

public sealed record LRequestRegisterShift(
    long LRequestDraftId, long LRequestCardId, long LRequestRegisterId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestRegisterShift);
}

public sealed record LRequestRegisterName(long LRequestDraftId, long LRequestRegisterId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestRegisterName);
}
