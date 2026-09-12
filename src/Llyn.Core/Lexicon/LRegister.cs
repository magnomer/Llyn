namespace Llyn.Core;

public sealed record LRegister(
    long LRegisterId,
    LStateValue LRegisterName,
    string LRegisterLanguage,
    long? LRegisterPackId = null)
{
    public LStateValue LRegisterName { get; init; } = LRegisterName ?? LStateValue.LStateValueUnspecified;

    public string LRegisterLanguage { get; init; } = LRegisterLanguage ?? string.Empty;

    public bool LRegisterBuiltin => LRegisterPackId is not null;
}
