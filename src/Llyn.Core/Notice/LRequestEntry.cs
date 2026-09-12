using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LRequestHeadword(long LRequestDraftId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestLanguage(long LRequestDraftId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestNote(long LRequestDraftId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestIpa(long LRequestDraftId, string LRequestText)
    : LRequest(LRequestDraftId);

public sealed record LRequestAudio(long LRequestDraftId, string LRequestFile, string? LRequestSource)
    : LRequest(LRequestDraftId);

public sealed record LRequestSpeech(long LRequestDraftId, IReadOnlyList<LSpeechDraft> LRequestSpeeches)
    : LRequest(LRequestDraftId);
