# PDirectoryItem.cs

## `internal sealed class PDirectoryItem`

Presentation item for one tag row in `PDirectory`.
Carries the id of the stored Tag, the text the row shows and whether that row is the chosen one.
The id is what the membership list is looked up by, and the text is only what is shown.
The chosen flag is read by the row template, which tints the tag the membership list currently stands on.
The chosen flag alone is settable and announces its change, so a new choice re-marks the rows in place.

## `private bool _pDirectoryItemChosen;`

Whether this row is the chosen one.

## `public event PropertyChangedEventHandler? PropertyChanged;`

Raised when the chosen flag moves, so the row template re-tints.

## `public bool PDirectoryItemChosen`

Whether this row is the chosen one, settable so the directory re-marks rows without rebuilding them.
