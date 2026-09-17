namespace Llyn.Core;

public sealed record LRequestPronunciationAddition(long LRequestDraftId, string LRequestText, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestPronunciationRemoval(long LRequestDraftId, long LRequestPronunciationId)
    : LRequest(LRequestDraftId);

public sealed record LRequestPronunciationShift(
    long LRequestDraftId, long LRequestPronunciationId, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestPronunciationIpa(long LRequestDraftId, long LRequestPronunciationId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestPronunciationIpa)}:{LRequestPronunciationId}";
}

public sealed record LRequestPronunciationRespelling(
    long LRequestDraftId, long LRequestPronunciationId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestPronunciationRespelling)}:{LRequestPronunciationId}";
}

public sealed record LRequestPronunciationVariety(
    long LRequestDraftId, long LRequestPronunciationId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestPronunciationVariety)}:{LRequestPronunciationId}";
}

public sealed record LRequestPronunciationAudio(
    long LRequestDraftId, long LRequestPronunciationId, string LRequestFile, string? LRequestSource)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestPronunciationAudio)}:{LRequestPronunciationId}";
}
