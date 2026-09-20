using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestGlossAddition(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, string LRequestLanguage, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestGlossAddition);
}

public sealed record LRequestGlossRemoval(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestGlossId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestGlossRemoval);
}

public sealed record LRequestGlossText(
    long LRequestDraftId,
    long LRequestCardId,
    long LRequestSentenceId,
    long LRequestGlossId,
    LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestGlossText)}:{LRequestGlossId}";
}

public sealed record LRequestGlossLanguage(
    long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestGlossId, string LRequestLanguage)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestGlossLanguage)}:{LRequestGlossId}";
}
