# PGamutItem.cs

## `internal sealed class PGamutItem`

One Register as a row of the tenor panel's catalog.
It carries the id the panel browses by, which is never the name, because a Register may be renamed.
It carries no language, because a Register belongs to none.
The count of cards marked with it is held as a number and shown as text.
A Register nothing is marked with shows nothing rather than a zero, because a zero reads as a value.
The chosen flag alone is settable and announces its change, so a new choice re-marks the rows in place.

## `private bool _pGamutItemChosen;`

Whether this row is the chosen one.

## `public event PropertyChangedEventHandler? PropertyChanged;`

Raised when the chosen flag moves, so the row template re-tints.

## `public bool PGamutItemChosen`

Whether this row is the chosen one, settable so the gamut re-marks rows without rebuilding them.
