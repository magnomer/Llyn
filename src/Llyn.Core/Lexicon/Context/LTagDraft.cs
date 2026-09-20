namespace Llyn.Core;

public sealed record LTagDraft(
    long LTagDraftId,
    string LTagDraftText)
{
    public string LTagDraftText { get; init; } = LTagDraftText ?? string.Empty;

    public bool LTagDraftStored => LTagDraftId != 0;

    public static LTagDraft LTagDraftCreate(string text)
    {
        return new LTagDraft(0, text);
    }
}
