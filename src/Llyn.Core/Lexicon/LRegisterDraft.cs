namespace Llyn.Core;

public sealed record LRegisterDraft(
    LStateValue LRegisterDraftName,
    long LRegisterDraftId,
    string LRegisterDraftLanguage = "",
    bool LRegisterDraftBuiltin = false)
{
    public LStateValue LRegisterDraftName { get; init; } =
        LRegisterDraftName ?? LStateValue.LStateValueUnspecified;

    public string LRegisterDraftLanguage { get; init; } = LRegisterDraftLanguage ?? string.Empty;

    public static LRegisterDraft LRegisterDraftCreate(string text)
    {
        return new LRegisterDraft(LStateValue.LStateValueRead(text), 0);
    }
}
