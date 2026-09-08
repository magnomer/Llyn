# LStateValue.cs

## `public sealed record LStateValue(`

A stored value together with what is known about it.
A field that shows nothing is not one thing.
Nothing may ever have been recorded there.
Or something may have been recorded that cannot be read back as itself.
It may be corrupted, or wrong where it stands.
The first is `LStateUnspecified`, the second `LStateUnknown`, and only `LStateSpecified` carries text.
Every field that can stand empty for either reason carries this rather than a bare string.
So the two are never stored, read, or shown as the same thing.

An unknown value keeps no text.
What was there could not be read, so there is nothing faithful to keep.
The state is the whole of what is known.

**Parameters**

- `LStateValueState` — Whether nothing was recorded, something unreadable was, or the value states text.
- `LStateValueText` — The text, and `null` unless the state is specified.

## `public static LStateValue LStateValueUnspecified { get; }`

The value nothing was ever recorded for.

## `public static LStateValue LStateValueUnknown { get; }`

The value something was recorded for that cannot be read back.

## `public static LStateValue LStateValueCreate(string text)`

The value stating `text`.

## `public static LStateValue LStateValueRead(string? text)`

Reads plain text as a value.
Text that is blank stands for nothing recorded.
Any other text stands for the value it states.
Nothing read this way is ever unknown.
A value that cannot be read back is something only its store or a later reading can find.
It is never something a form produces by leaving a box empty.

## `public static implicit operator LStateValue(string? text)`

Lets plain text stand where a value is wanted, reading it the way `LStateValueRead` does.
Only the writing side converts.
A value never turns back into a string on its own.
So no reader can lose the distinction by accident.

## `public static LStateValue LStateValueResolve(string? text, bool unreadable)`

Reads a form field as a value.
The text it holds and the mark that says it cannot be read are both used.
This is the one rule for the three states, and every editor asks it rather than deciding for itself.

The mark decides first.
A marked field is unknown whatever its box holds, because an unknown value keeps no text.
Every editor empties the box when the mark goes on, so nothing written is lost by this order.

An unmarked field is read the way `LStateValueRead` reads it.
Text that is blank or nothing but whitespace stands for nothing recorded.
A field holding only spaces is therefore unspecified and not text.
So a value written this way, stored, and read back maps to itself.
An editor comparing what it sent against what is held sees no difference and does not report itself dirty.

## `public string LStateValueShow()`

The text to show for the value, which is nothing at all unless the value states one.
A caller that must tell an unknown value from an empty one reads the state instead.

## `public bool LStateValueEmpty`

Whether nothing was ever recorded.
An unknown value is not empty by this reading: something is there, and only its text is beyond reach.
