namespace Llyn.Core;

public sealed record LRequestReferenceTitle(long LRequestDraftId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceYear(long LRequestDraftId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceKind(long LRequestDraftId, LReferenceKind LRequestKind)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceNote(long LRequestDraftId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceUrl(long LRequestDraftId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);
