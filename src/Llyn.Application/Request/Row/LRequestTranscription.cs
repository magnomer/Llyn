namespace Llyn.Application;

public sealed record LRequestTranscriptionAddition(
    long LRequestDraftId, string LRequestScheme, int LRequestPosition, bool LRequestSeeded = false)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTranscriptionAddition);
}

public sealed record LRequestTranscriptionRemoval(long LRequestDraftId, long LRequestTranscriptionId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTranscriptionRemoval);
}

public sealed record LRequestTranscriptionShift(
    long LRequestDraftId, long LRequestTranscriptionId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestTranscriptionShift);
}

public sealed record LRequestTranscriptionScheme(
    long LRequestDraftId, long LRequestTranscriptionId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestTranscriptionScheme)}:{LRequestTranscriptionId}";
}

public sealed record LRequestTranscriptionText(long LRequestDraftId, long LRequestTranscriptionId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestTranscriptionText)}:{LRequestTranscriptionId}";
}
