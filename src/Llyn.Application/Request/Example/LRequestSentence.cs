using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestSentenceAddition(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSentenceAddition);
}

public sealed record LRequestSentenceRemoval(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestSentenceId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSentenceRemoval);
}

public sealed record LRequestSentenceShift(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestSentenceId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestSentenceShift);
}

public sealed record LRequestSentenceExample(
    long LRequestDraftId, LCardKind LRequestKind, long LRequestCardId, long LRequestSentenceId, long LRequestExampleId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey =>
        $"{nameof(LRequestSentenceExample)}:{LRequestKind}:{LRequestCardId}:{LRequestSentenceId}";
}

public sealed record LRequestSentenceText(
    long LRequestDraftId,
    LCardKind LRequestKind,
    long LRequestCardId,
    long LRequestSentenceId,
    LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey =>
        $"{nameof(LRequestSentenceText)}:{LRequestKind}:{LRequestCardId}:{LRequestSentenceId}";
}

public sealed record LRequestSentenceParticle(
    long LRequestDraftId,
    LCardKind LRequestKind,
    long LRequestCardId,
    long LRequestSentenceId,
    LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey =>
        $"{nameof(LRequestSentenceParticle)}:{LRequestKind}:{LRequestCardId}:{LRequestSentenceId}";
}

public sealed record LRequestSentenceDependence(
    long LRequestDraftId,
    LCardKind LRequestKind,
    long LRequestCardId,
    long LRequestSentenceId,
    LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey =>
        $"{nameof(LRequestSentenceDependence)}:{LRequestKind}:{LRequestCardId}:{LRequestSentenceId}";
}

public sealed record LRequestSentenceReference(
    long LRequestDraftId,
    LCardKind LRequestKind,
    long LRequestCardId,
    long LRequestSentenceId,
    long LRequestReferenceId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey =>
        $"{nameof(LRequestSentenceReference)}:{LRequestKind}:{LRequestCardId}:{LRequestSentenceId}";
}
