using System;

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

    public string? LStateValueShown => LStateValueShow() is { Length: > 0 } shown ? shown : null;

    public bool LStateValueUncertain => LStateValueState == LState.LStateUnknown;

    public bool LStateValueMatch(string text)
    {
        return string.Equals(LStateValueShow(), text, StringComparison.Ordinal);
    }

    public bool LStateValueMatch(LStateValue? other)
    {
        return Equals(other);
    }

    public bool LStateValueLegible => LStateValueState == LState.LStateSpecified || LStateValueUnreadable;

    public bool LStateValueSound =>
        LStateValueState == LState.LStateSpecified && !LStateValueUnreadable && LStateValueShown is not null;

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
