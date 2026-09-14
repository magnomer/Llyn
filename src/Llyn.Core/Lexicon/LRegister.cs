namespace Llyn.Core;

public sealed record LRegister(
    long LRegisterId,
    LStateValue LRegisterName,
    bool LRegisterBuiltin = false)
{
    public LStateValue LRegisterName { get; init; } = LRegisterName ?? LStateValue.LStateValueUnspecified;
}
