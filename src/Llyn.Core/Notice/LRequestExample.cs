namespace Llyn.Core;

public sealed record LRequestExampleText(long LRequestDraftId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestExampleTranslation(long LRequestDraftId, LStateValue LRequestValue)
    : LRequest(LRequestDraftId);

public sealed record LRequestExampleLanguage(long LRequestDraftId, string LRequestLanguage)
    : LRequest(LRequestDraftId);

public sealed record LRequestExampleReference(long LRequestDraftId, long LRequestReferenceId)
    : LRequest(LRequestDraftId);

public sealed record LRequestExampleBody(long LRequestDraftId, LExample LRequestExample)
    : LRequest(LRequestDraftId);
