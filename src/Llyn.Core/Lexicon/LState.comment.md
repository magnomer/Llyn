# LState.cs

## `public enum LState`

The three-state distinction a stored value can carry: whether it was never given, was deliberately marked as not known, or holds a real value. Source metadata (job10) and any other field that must tell "no value yet" apart from "known to be absent" use this.

## `LStateUnspecified,`

No value has been supplied.

## `LStateUnknown,`

A value was deliberately marked as not known.

## `LStateSpecified,`

A real value is present.
