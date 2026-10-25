using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestGlossAddition(
    long LRequestDraftId, LExampleSite LRequestSite, string LRequestLanguage, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestGlossAddition);
}

public sealed record LRequestGlossRemoval(long LRequestDraftId, LExampleSite LRequestSite, long LRequestGlossId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestGlossRemoval);
}

public sealed record LRequestGlossText(
    long LRequestDraftId, LExampleSite LRequestSite, long LRequestGlossId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey =>
        $"{nameof(LRequestGlossText)}:{LRequestSite.LExampleSiteKey}:{LRequestGlossId}";
}

public sealed record LRequestGlossLanguage(
    long LRequestDraftId, LExampleSite LRequestSite, long LRequestGlossId, string LRequestLanguage)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey =>
        $"{nameof(LRequestGlossLanguage)}:{LRequestSite.LExampleSiteKey}:{LRequestGlossId}";
}
