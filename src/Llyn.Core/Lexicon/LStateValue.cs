namespace Llyn.Core;

public sealed record LStateValue(
    LState LStateValueState,
    string? LStateValueText,
    bool LStateValueUnreadable = false)
{
    public static LStateValue LStateValueUnspecified { get; } = new(LState.LStateUnspecified, null);

    public static LStateValue LStateValueUnknown { get; } = new(LState.LStateUnknown, null);

    public static LStateValue LStateValueCreate(string text)
    {
        return new LStateValue(LState.LStateSpecified, text);
    }

    public static LStateValue LStateValueRead(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? LStateValueUnspecified : LStateValueCreate(text);
    }

    public string LStateValueShow()
    {
        return LStateValueText is not null && (LStateValueState == LState.LStateSpecified || LStateValueUnreadable)
            ? LStateValueText
            : string.Empty;
    }

    public static implicit operator LStateValue(string? text)
    {
        return LStateValueRead(text);
    }

    public LStateValue LStateValueNormalize()
    {
        return LStateValueUnreadable ? LStateValueUnspecified : this;
    }

    public bool LStateValueEmpty => LStateValueState == LState.LStateUnspecified;
}
