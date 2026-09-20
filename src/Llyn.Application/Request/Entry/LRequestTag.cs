namespace Llyn.Application;

public sealed record LRequestTagAddition(
    long LRequestDraftId, long LRequestCardId, string LRequestText, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTagAddition);
}

public sealed record LRequestTagPick(
    long LRequestDraftId, long LRequestCardId, long LRequestTagId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTagPick);
}

public sealed record LRequestTagRemoval(long LRequestDraftId, long LRequestCardId, long LRequestTagId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTagRemoval);
}

public sealed record LRequestTagShift(
    long LRequestDraftId, long LRequestCardId, long LRequestTagId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTagShift);
}

public sealed record LRequestTagText(long LRequestDraftId, long LRequestTagId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTagText);
}
