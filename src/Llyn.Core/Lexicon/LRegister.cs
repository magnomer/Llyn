namespace Llyn.Core;

public sealed record LRegister(
    long LRegisterId,
    LStateValue LRegisterName,
    string LRegisterLanguage,
    bool LRegisterBuiltin,
    string? LRegisterPackKey = null)
{
    public LStateValue LRegisterName { get; init; } = LRegisterName ?? LStateValue.LStateValueUnspecified;

    public string LRegisterLanguage { get; init; } = LRegisterLanguage ?? string.Empty;
}
