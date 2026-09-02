# LStateValue.cs

## `public sealed record LStateValue(`

A stored value together with what is known about it. A field that shows nothing is not one thing: nothing may ever have been recorded there, or something may have been recorded that cannot be read back as itself — corrupted, or wrong where it stands. The first is `LStateUnspecified`, the second `LStateUnknown`, and only `LStateSpecified` carries text. Every field that can stand empty for either reason carries this rather than a bare string, so the two are never stored, read, or shown as the same thing.

An unknown value keeps no text. What was there could not be read, so there is nothing faithful to keep; the state is the whole of what is known.

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

Reads plain text as a value: text that is blank stands for nothing recorded, and any other text for the value it states. Nothing read this way is ever unknown — a value that cannot be read back is something only its store or a later reading can find, never something a form produces by leaving a box empty.

## `public static implicit operator LStateValue(string? text)`

Lets plain text stand where a value is wanted, reading it the way `LStateValueRead` does. Only the writing side converts: a value never turns back into a string on its own, so no reader can lose the distinction by accident.

## `public string LStateValueShow()`

The text to show for the value, which is nothing at all unless the value states one. A caller that must tell an unknown value from an empty one reads the state instead.

## `public bool LStateValueEmpty`

Whether nothing was ever recorded. An unknown value is not empty by this reading: something is there, and only its text is beyond reach.
