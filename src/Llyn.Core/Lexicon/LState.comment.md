# LState.cs

## `public enum LState`

The three-state distinction a stored value can carry.
It says whether the value was never given, was marked as not known, or holds a real value.
Source metadata (job10) uses this, and so does any other field with the same need.
Such a field must tell "no value yet" apart from "known to be absent".

## `LStateUnspecified,`

No value has been supplied.

## `LStateUnknown,`

A value was deliberately marked as not known.

## `LStateSpecified,`

A real value is present.
