# LStateValue.cs

## `public sealed record LStateValue(`

A stored value together with what is known about it.
A field that shows nothing is not one thing.
Nothing may ever have been recorded there.
Or the user may have marked it as not known.
It may be corrupted, or wrong where it stands.
The first is `LStateUnspecified`, the second `LStateUnknown`, and only `LStateSpecified` carries text.
Every field that can stand empty for either reason carries this rather than a bare string.
So the two are never stored, read, or shown as the same thing.

An unknown value keeps no text.
The user chose not to give one, so there is nothing to keep.
The state is the whole of what is known.

A fourth case is not a state but a diagnosis.
The store may hold a state word the engine cannot read.
Such a value is unreadable.
Its state stands unspecified, it keeps the stored text for showing, and it can never be written back.
Only a store reading produces it, never a form.

**Parameters**

- `LStateValueState` — Whether nothing was recorded, the user marked it unknown, or the value states text.
- `LStateValueText` — The text, and `null` unless the state is specified or the value is unreadable.
- `LStateValueUnreadable` — Whether the store held something the engine could not read.

## `public static LStateValue LStateValueUnspecified { get; }`

The value nothing was ever recorded for.

## `public static LStateValue LStateValueUnknown { get; }`

The value the user marked as not known.

## `public static LStateValue LStateValueCreate(string text)`

The value stating `text`.

## `public static LStateValue LStateValueRead(string? text)`

Reads plain text as a value.
Text that is blank stands for nothing recorded.
Any other text stands for the value it states.
Nothing read this way is ever unknown.
Only the user can mark a value as not known, so plain text never turns into one.
It is never something a form produces by leaving a box empty.

## `public static implicit operator LStateValue(string? text)`

Lets plain text stand where a value is wanted, reading it the way `LStateValueRead` does.
Only the writing side converts.
A value never turns back into a string on its own.
So no reader can lose the distinction by accident.

## `public string LStateValueShow()`

The text to show for the value, which is nothing at all unless the value states one or is unreadable.
A caller that must tell an unknown value from an empty one reads the state instead.

## `public bool LStateValueEmpty`

Whether nothing was ever recorded.
An unknown value is not empty by this reading: something is there, and only its text is beyond reach.

## `public LStateValue LStateValueNormalize()`

This value when it is readable, and the unspecified value when it is not.
The stored text an unreadable value carried is lost here.
That is why only the engine calls it, and only after the user agreed.
