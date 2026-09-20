# LStateMark.cs

## `public sealed record LStateMark`

A stated field with no payload beside it: the three-way state alone.

`LStateValue` carries text and `LStateAnchor` carries an id.
This carries a field whose content lives in another table, so nothing sits next to the state.
The author credits of a source are such a field.
The names are rows of `reference_author`, and the state is one column.
A bare `LState` could hold that column but could not say when its stored word was unreadable.
`LStateMarkUnreadable` marks a state whose stored word the engine could not read.
Such a mark is shown but refused on write until the user clears it.

## `public static LStateMark LStateMarkRead(LState state)`

The readable mark for a state the interface or a request supplies.

## `public LStateMark LStateMarkNormalize()`

This mark when it is readable, and the unspecified mark when it is not.
