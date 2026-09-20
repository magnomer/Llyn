namespace Llyn.Core;

public sealed record LStateAnchor(
    LState LStateAnchorState,
    long? LStateAnchorId,
    bool LStateAnchorUnreadable = false)
{
    public static LStateAnchor LStateAnchorUnspecified { get; } = new(LState.LStateUnspecified, null);

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

    public long? LStateAnchorShown => LStateAnchorShow() is not 0 and long id ? id : null;

    public bool LStateAnchorLinked => LStateAnchorShow() != 0;

    public bool LStateAnchorMatch(long id)
    {
        return LStateAnchorShown == id;
    }

    public LStateAnchor LStateAnchorNormalize()
    {
        return LStateAnchorUnreadable ? LStateAnchorUnspecified : this;
    }

    public bool LStateAnchorEmpty => LStateAnchorState == LState.LStateUnspecified;
}
