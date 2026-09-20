namespace Llyn.Application;

public sealed record LRequestTranslationPick(
    long LRequestDraftId, long LRequestCardId, long LRequestEntryId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTranslationPick);
}

public sealed record LRequestTranslationRemoval(long LRequestDraftId, long LRequestCardId, long LRequestEntryId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTranslationRemoval);
}

public sealed record LRequestTranslationShift(
    long LRequestDraftId, long LRequestCardId, long LRequestEntryId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTranslationShift);
}
