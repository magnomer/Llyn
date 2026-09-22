namespace Llyn.Application;

public sealed record LRequestEtymologyText(long LRequestDraftId, string LRequestText)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestEtymologyText);
}

public sealed record LRequestEtymologyMention(
    long LRequestDraftId, int LRequestOffset, int LRequestLength, long LRequestEntryId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestEtymologyMention)}:{LRequestOffset}";
}

public sealed record LRequestEtymonAddition(long LRequestDraftId, long LRequestEntryId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestEtymonAddition);
}

public sealed record LRequestEtymonRemoval(long LRequestDraftId, long LRequestEntryId)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => nameof(LRequestEtymonRemoval);
}

public sealed record LRequestEtymonShift(long LRequestDraftId, long LRequestEntryId, int LRequestPosition)
    : LRequest(LRequestDraftId)
{
    public override string LRequestKey => $"{nameof(LRequestEtymonShift)}:{LRequestEntryId}";
}
