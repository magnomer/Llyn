namespace Llyn.Application;

public sealed record LRequestAncestorText(long LRequestDraftId, long LRequestAncestorId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestAncestorText)}:{LRequestAncestorId}";
}

public sealed record LRequestAncestorTarget(long LRequestDraftId, long LRequestAncestorId, long LRequestEntryId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAncestorTarget);
}

public sealed record LRequestAncestorLanguage(
    long LRequestDraftId, long LRequestAncestorId, string LRequestText, long LRequestTongueId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestAncestorLanguage)}:{LRequestAncestorId}";
}

public sealed record LRequestAncestorGloss(long LRequestDraftId, long LRequestAncestorId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestAncestorGloss)}:{LRequestAncestorId}";
}

public sealed record LRequestAncestorRelation(long LRequestDraftId, long LRequestAncestorId, long LRequestRelationId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestAncestorRelation)}:{LRequestAncestorId}";
}

public sealed record LRequestAncestorInsertion(long LRequestDraftId, long LRequestAncestorId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAncestorInsertion);
}

public sealed record LRequestAncestorAddition(long LRequestDraftId, long LRequestAncestorId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAncestorAddition);
}

public sealed record LRequestAncestorRemoval(long LRequestDraftId, long LRequestAncestorId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAncestorRemoval);
}

public sealed record LRequestAncestorRestoration(long LRequestDraftId, long LRequestAncestorId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestAncestorRestoration);
}
