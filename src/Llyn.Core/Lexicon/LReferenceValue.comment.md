# LReferenceValue.cs

## `public sealed record LReferenceValue(`

One state-bearing field of a `LReference`: the three-state `LReferenceValueState` together with the text it carries. The state, not the text, says what the field means — `LState.LStateUnspecified` and `LState.LStateUnknown` both hold no text and are still different facts ("nothing has been entered" versus "recorded as not knowable"), while `LState.LStateSpecified` is the only state carrying a value.

A specified value is ordinary text with no reserved readings: `Untitled` is a title someone chose, not an unspecified title.

**Parameters**

- `LReferenceValueState` — Whether the field is unspecified, unknown, or specified.
- `LReferenceValueText` — The text when specified; `null` in every other state.

## `public static LReferenceValue LReferenceValueUnspecified { get; } =`

The field nobody has filled in yet: unspecified, carrying no text.

## `public static LReferenceValue LReferenceValueUnknown { get; } = new(LState.LStateUnknown, null);`

The field deliberately recorded as not knowable: unknown, carrying no text.

## `public static LReferenceValue LReferenceValueCreate(string text)`

Builds a specified field carrying `text`.
