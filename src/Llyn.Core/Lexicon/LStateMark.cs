namespace Llyn.Core;

public sealed record LStateMark(
    LState LStateMarkState,
    bool LStateMarkUnreadable = false)
{
    public static LStateMark LStateMarkUnspecified { get; } = new(LState.LStateUnspecified);

    public static LStateMark LStateMarkUnknown { get; } = new(LState.LStateUnknown);

    public static LStateMark LStateMarkSpecified { get; } = new(LState.LStateSpecified);

    public static LStateMark LStateMarkRead(LState state)
    {
        return new LStateMark(state);
    }

    public LStateMark LStateMarkNormalize()
    {
        return LStateMarkUnreadable ? LStateMarkUnspecified : this;
    }

    public bool LStateMarkEmpty => LStateMarkState == LState.LStateUnspecified;

    public bool LStateMarkUncertain => LStateMarkState == LState.LStateUnknown;
}
