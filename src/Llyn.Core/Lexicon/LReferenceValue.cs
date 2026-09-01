namespace Llyn.Core;

/// <summary>
/// One state-bearing field of a <see cref="LReference"/>: the three-state
/// <see cref="LReferenceValueState"/> together with the text it carries. The state, not the text,
/// says what the field means — <see cref="LState.LStateUnspecified"/> and
/// <see cref="LState.LStateUnknown"/> both hold no text and are still different facts ("nothing has
/// been entered" versus "recorded as not knowable"), while
/// <see cref="LState.LStateSpecified"/> is the only state carrying a value.
/// <para>
/// A specified value is ordinary text with no reserved readings: <c>Untitled</c> is a title someone
/// chose, not an unspecified title.
/// </para>
/// </summary>
/// <param name="LReferenceValueState">Whether the field is unspecified, unknown, or specified.</param>
/// <param name="LReferenceValueText">The text when specified; <c>null</c> in every other state.</param>
public sealed record LReferenceValue(
    LState LReferenceValueState,
    string? LReferenceValueText)
{
    /// <summary>The field nobody has filled in yet: unspecified, carrying no text.</summary>
    public static LReferenceValue LReferenceValueUnspecified { get; } =
        new(LState.LStateUnspecified, null);

    /// <summary>The field deliberately recorded as not knowable: unknown, carrying no text.</summary>
    public static LReferenceValue LReferenceValueUnknown { get; } = new(LState.LStateUnknown, null);

    /// <summary>Builds a specified field carrying <paramref name="text"/>.</summary>
    public static LReferenceValue LReferenceValueCreate(string text)
    {
        return new LReferenceValue(LState.LStateSpecified, text);
    }
}
