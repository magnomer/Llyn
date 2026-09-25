namespace Llyn.Core;

public sealed record LStateWritten(
    string? LStateWrittenText,
    bool LStateWrittenUnknown = false)
{
    public static LStateWritten LStateWrittenEmpty { get; } = new(null, false);

    public LStateValue LStateWrittenResolve()
    {
        return LStateWrittenUnknown ? LStateValue.LStateValueUnknown : LStateValue.LStateValueRead(LStateWrittenText);
    }

    public bool LStateWrittenMatch(LStateValue? value)
    {
        return LStateWrittenResolve() == (value ?? LStateValue.LStateValueUnspecified);
    }
}
