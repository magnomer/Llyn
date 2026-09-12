namespace Llyn.Core;

public sealed record LStateWritten(
    string? LStateWrittenText,
    bool LStateWrittenUnknown = false)
{
    public static LStateWritten LStateWrittenEmpty { get; } = new(null);

    public static LStateWritten LStateWrittenRead(LStateValue value)
    {
        return value is null
            ? LStateWrittenEmpty
            : new LStateWritten(value.LStateValueShow(), value.LStateValueState == LState.LStateUnknown);
    }

    public LStateValue LStateWrittenResolve()
    {
        return LStateWrittenUnknown ? LStateValue.LStateValueUnknown : LStateValue.LStateValueRead(LStateWrittenText);
    }

    public bool LStateWrittenMatch(LStateValue? value)
    {
        return LStateWrittenResolve() == (value ?? LStateValue.LStateValueUnspecified);
    }
}
