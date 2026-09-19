using Llyn.Core;

namespace Llyn.Application;

public sealed record LRequestExampleText(long LRequestDraftId, LStateWritten LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestExampleLanguage(long LRequestDraftId, string LRequestLanguage)
    : LRequest(LRequestDraftId);

public sealed record LRequestExampleReference(long LRequestDraftId, long LRequestReferenceId)
    : LRequest(LRequestDraftId);

public sealed record LRequestExampleBody(
    long LRequestDraftId,
    string? LRequestLanguage,
    LStateWritten LRequestText,
    long LRequestReferenceId)
    : LRequest(LRequestDraftId);
