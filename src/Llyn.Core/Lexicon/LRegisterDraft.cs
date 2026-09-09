namespace Llyn.Core;

public sealed record LRegisterDraft(
    LStateValue LRegisterDraftText,
    string LRegisterDraftId)
{
    public LStateValue LRegisterDraftText { get; init; } =
        LRegisterDraftText ?? LStateValue.LStateValueUnspecified;

    public static LRegisterDraft LRegisterDraftCreate(string text)
    {
        return new LRegisterDraft(LStateValue.LStateValueRead(text), string.Empty);
    }
}
