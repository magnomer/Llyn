namespace Llyn.Core;

public sealed record LRequestReferenceTitle(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceYear(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceKind(long LRequestDraftId, LReferenceKind LRequestKind)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceNote(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceUrl(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestReferenceBody(
    long LRequestDraftId,
    LStateWritten LRequestTitle,
    LStateWritten LRequestYear,
    LReferenceKind LRequestKind,
    LStateWritten LRequestNote,
    LStateWritten LRequestUrl,
    LState LRequestAuthorState)
    : LRequest(LRequestDraftId);
