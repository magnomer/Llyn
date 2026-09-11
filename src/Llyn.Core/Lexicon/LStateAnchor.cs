namespace Llyn.Core;

public sealed record LStateAnchor(
    LState LStateAnchorState,
    long? LStateAnchorId)
{
    public static LStateAnchor LStateAnchorUnspecified { get; } = new(LState.LStateUnspecified, null);

    public static LStateAnchor LStateAnchorUnknown { get; } = new(LState.LStateUnknown, null);

    public static LStateAnchor LStateAnchorCreate(long id)
    {
        return new LStateAnchor(LState.LStateSpecified, id);
    }

    public static LStateAnchor LStateAnchorRead(long? id)
    {
        return id is null or 0 ? LStateAnchorUnspecified : LStateAnchorCreate(id.Value);
    }

    public long LStateAnchorShow()
    {
        return LStateAnchorState == LState.LStateSpecified && LStateAnchorId is not null
            ? LStateAnchorId.Value
            : 0;
    }

    public bool LStateAnchorEmpty => LStateAnchorState == LState.LStateUnspecified;
}
