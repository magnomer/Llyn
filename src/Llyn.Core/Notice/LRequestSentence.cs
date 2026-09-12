namespace Llyn.Core;

public sealed record LRequestSentenceAddition(long LRequestDraftId, long LRequestCardId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSentenceRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSentenceId)
    : LRequest(LRequestDraftId);

public sealed record LRequestSentenceShift(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestSentenceExample(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestExampleId)
    : LRequest(LRequestDraftId);

public sealed record LRequestSentenceText(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSentenceParticle(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSentenceDependence(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestSentenceReference(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestReferenceId)
    : LRequest(LRequestDraftId);
