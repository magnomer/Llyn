# QSCustomsItem.cs
Hash: `5bff43c2c4aa264d`

## `internal sealed class QSCustomsItem`

One row of the customs window: an entry from the file, as the window shows it.
It keeps the entry's position in the file, since the gate and the intake address rows by position.
The candidates are the ids of the stored entries sharing its headword, found by the engine before the question.
Its mode and target live in `CSCustoms`, so the row holds nothing the user changes.

## `public string QSCustomsItemNumber`

The row's place in the file, counted from one as a reader counts.

## `public string QSCustomsItemHeadword`

The headword as shown, fixed when the row is made.
Every candidate shares it, so the target dropdown shows it beside each id.
