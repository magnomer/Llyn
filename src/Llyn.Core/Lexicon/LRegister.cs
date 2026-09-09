namespace Llyn.Core;

public sealed record LRegister(
    string LRegisterId,
    LStateValue LRegisterName,
    string LRegisterLanguage,
    bool LRegisterBuiltin)
{
    public LStateValue LRegisterName { get; init; } = LRegisterName ?? LStateValue.LStateValueUnspecified;

    public string LRegisterLanguage { get; init; } = LRegisterLanguage ?? string.Empty;
}
