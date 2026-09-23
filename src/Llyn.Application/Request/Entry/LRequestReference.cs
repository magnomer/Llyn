using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestReferenceTitle(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestReferenceTitle);
}

public sealed record LRequestReferenceYear(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestReferenceYear);
}

public sealed record LRequestReferenceKind(long LRequestDraftId, LReferenceKind LRequestKind)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestReferenceKind);
}

public sealed record LRequestReferenceNote(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestReferenceNote);
}

public sealed record LRequestReferenceUrl(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestReferenceUrl);
}
