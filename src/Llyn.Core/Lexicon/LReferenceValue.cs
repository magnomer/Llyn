namespace Llyn.Core;

public sealed record LReferenceValue(
    LState LReferenceValueState,
    string? LReferenceValueText)
{
    public static LReferenceValue LReferenceValueUnspecified { get; } =
        new(LState.LStateUnspecified, null);

    public static LReferenceValue LReferenceValueUnknown { get; } = new(LState.LStateUnknown, null);

    public static LReferenceValue LReferenceValueCreate(string text)
    {
        return new LReferenceValue(LState.LStateSpecified, text);
    }
}
