namespace Llyn.Core;

public sealed record LRequestTranslationPick(
    long LRequestDraftId, long LRequestCardId, long LRequestEntryId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestTranslationRemoval(long LRequestDraftId, long LRequestCardId, long LRequestEntryId)
    : LRequest(LRequestDraftId);

public sealed record LRequestTranslationShift(
    long LRequestDraftId, long LRequestCardId, long LRequestEntryId, int LRequestPosition)
    : LRequest(LRequestDraftId);
