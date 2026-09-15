namespace Llyn.Core;

public sealed record LRequestReflexAddition(
    long LRequestDraftId, string LRequestLanguage, string LRequestKind, int LRequestPosition)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexRemoval(long LRequestDraftId, long LRequestReflexId)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexLanguage(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexKind(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexText(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexRespelling(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexNote(long LRequestDraftId, long LRequestReflexId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestReflexMain(long LRequestDraftId, long LRequestReflexId, bool LRequestMain)
    : LRequest(LRequestDraftId);
