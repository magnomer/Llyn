namespace Llyn.Core;

public sealed record LRegisterDraft(
    LStateValue LRegisterDraftName,
    long LRegisterDraftId)
{
    public LStateValue LRegisterDraftName { get; init; } =
        LRegisterDraftName ?? LStateValue.LStateValueUnspecified;

    public LRegisterDraft LRegisterDraftNormalize()
    {
        return this with { LRegisterDraftName = LRegisterDraftName.LStateValueNormalize() };
    }

    public static LRegisterDraft LRegisterDraftCreate(string text)
    {
        return new LRegisterDraft(LStateValue.LStateValueRead(text), 0);
    }
}
