namespace Llyn.Core;

public sealed record LRequestReflexAddition(
    long LRequestDraftId, string LRequestLanguage, string LRequestKind, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexRemoval(long LRequestDraftId, long LRequestReflexId)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexLanguage(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexLanguage)}:{LRequestReflexId}";
}

public sealed record LRequestReflexKind(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexKind)}:{LRequestReflexId}";
}

public sealed record LRequestReflexText(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexText)}:{LRequestReflexId}";
}

public sealed record LRequestReflexRespelling(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexRespelling)}:{LRequestReflexId}";
}

public sealed record LRequestReflexNote(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexNote)}:{LRequestReflexId}";
}

public sealed record LRequestReflexMain(long LRequestDraftId, long LRequestReflexId, bool LRequestMain)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexMain)}:{LRequestReflexId}";
}

public sealed record LRequestReflexRemark(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexRemark)}:{LRequestReflexId}";
}

public sealed record LRequestReflexAnchor(
    long LRequestDraftId, long LRequestReflexId, long LRequestFanqieId, bool LRequestAnchored)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestReflexAnchor)}:{LRequestReflexId}";
}
