# PXiaoyunItem.cs

## `internal sealed class PXiaoyunItem`

One Entry as a row of the yunjing panel's entry list, written with a character at the chosen cell.
It mirrors the tenor panel's row: flag, headword and the shown name, numbered apart when headwords twin.
The mark saying which row the reader stands on is the one thing that changes after the row is built.

## `public string PXiaoyunItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.
