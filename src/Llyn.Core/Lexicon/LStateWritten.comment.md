# LStateWritten.cs

## `public sealed record LStateWritten`

A form field as the user left it: the text in the box and the mark that says the value is not known.

It carries no state, because the shell never decides what a field means.
The shell sends what was written, the engine resolves it, and the shell shows what the engine holds.
So a request carries one of these where a stored value would otherwise be built on the wrong side.

**Parameters**

- `LStateWrittenText` — The text in the box, `null` when the box is absent.
- `LStateWrittenUnknown` — Whether the unknown mark is on.

## `public static LStateWritten LStateWrittenEmpty { get; }`

A field with nothing written and no mark.

## `public static LStateWritten LStateWrittenRead(LStateValue value)`

The field a stored value stands as: its shown text, and the mark when it is unknown.

## `public LStateValue LStateWrittenResolve()`

Reads the field as a value.
This is the one rule for the three states, and it runs on the engine side.

The mark decides first.
A marked field is unknown whatever its box holds, because an unknown value keeps no text.
Every editor empties the box when the mark goes on, so nothing written is lost by this order.
An unmarked field that is blank stands for nothing recorded.
Any other text is the value it states.

## `public bool LStateWrittenMatch(LStateValue? value)`

Whether the field already stands for `value`, so an editor can leave a box the user is typing in alone.
A missing value counts as nothing recorded.
