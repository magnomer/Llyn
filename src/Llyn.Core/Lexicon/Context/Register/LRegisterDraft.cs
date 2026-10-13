namespace Llyn.Core;

public sealed record LRegisterDraft(
    LStateValue LRegisterDraftName,
    long LRegisterDraftId)
{
    public LStateValue LRegisterDraftName { get; init; } =
        LRegisterDraftName ?? LStateValue.LStateValueUnspecified;

    public bool LRegisterDraftStored => LRegisterDraftId != 0;

    public LRegisterDraft LRegisterDraftNormalize()
    {
        return this with { LRegisterDraftName = LRegisterDraftName.LStateValueNormalize() };
    }
}
