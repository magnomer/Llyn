using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestExampleText(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestExampleText);
}

public sealed record LRequestExampleLanguage(long LRequestDraftId, string LRequestLanguage)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestExampleLanguage);
}

public sealed record LRequestExampleReference(long LRequestDraftId, long LRequestReferenceId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestExampleReference);
}
