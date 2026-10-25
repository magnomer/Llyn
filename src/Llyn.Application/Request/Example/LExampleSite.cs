using Llyn.Core;

namespace Llyn.Application;

public abstract record LExampleSite
{
    public abstract string LExampleSiteKey { get; }

    public abstract LExampleDraft? LExampleSiteRead(LDraft? draft);
}

public sealed record LExampleSiteCard(LCardKind LExampleSiteKind, long LExampleSiteHolder, long LExampleSiteSentence)
    : LExampleSite
{
    public override string LExampleSiteKey => $"{LExampleSiteKind}:{LExampleSiteHolder}:{LExampleSiteSentence}";

    public override LExampleDraft? LExampleSiteRead(LDraft? draft)
    {
        return draft?.LDraftExampleRead(LExampleSiteKind, LExampleSiteHolder, LExampleSiteSentence);
    }
}

public sealed record LExampleSiteDraft : LExampleSite
{
    public override string LExampleSiteKey => nameof(LExampleSiteDraft);

    public override LExampleDraft? LExampleSiteRead(LDraft? draft)
    {
        return draft?.LDraftExampleRead();
    }
}
