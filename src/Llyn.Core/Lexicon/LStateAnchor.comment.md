# LStateAnchor.cs

## `public sealed record LStateAnchor`

A stated link to another row: the three-way state, and the id it points at.

`LStateValue` carries a field the user typed, so its payload is text.
This carries a field that names another row, so its payload is an id and never text.
Both are needed because a link slot has the same three answers a text slot does.
"No source was given" and "we looked and the source is unknown" stay different facts.

`LStateAnchorId` is filled only when the state is specified.
An unspecified or unknown anchor points at nothing, which is why the id is nullable.
`LStateAnchorUnreadable` marks an anchor whose stored state word the engine could not read.
Such an anchor is shown but refused on write until the user clears it.

## `public static LStateAnchor LStateAnchorCreate(long id)`

The anchor for a row that is known: state specified, pointing at `id`.

## `public static LStateAnchor LStateAnchorRead(long? id)`

Reads an anchor from a stored id, treating a missing id and a zero id alike as unspecified.

Zero is never a real row id, because SQLite counts from one.
So it is the value a record carries before anything has been linked.

## `public long LStateAnchorShow()`

The id this anchor points at, or zero when it points at nothing.

Callers that only want to follow the link get one value to test rather than a state and an id.

## `public LStateAnchor LStateAnchorNormalize()`

This anchor when it is readable, and the unspecified anchor when it is not.
