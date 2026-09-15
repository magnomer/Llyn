namespace Llyn.Core;

public sealed record LRequestGlossAddition(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, string LRequestLanguage, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestGlossRemoval(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestGlossId)
    : LRequest(LRequestDraftId);

public sealed record LRequestGlossText(
    long LRequestDraftId,
    long LRequestCardId,
    long LRequestSentenceId,
    long LRequestGlossId,
    LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestGlossLanguage(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestGlossId, string LRequestLanguage)
    : LRequest(LRequestDraftId);
