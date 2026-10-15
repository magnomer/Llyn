# Sheaf

The drivers of the chosen deck's cue listing, its adder and the free-cue cell editor.

## `QCue`

Drives the listing of the chosen deck on the same `PDeck` page.
`CCueChanged`, `CDeckChanged` and `CAccessionChanged` repaint it from one `CCueRead`, and an empty read paints no rows.
Each row shows `CCueRowLine`, the flag and excluded boxes, and the orphan notice when the row is orphaned.
  The splice keeps the held rows when id, position and orphan state match.
  Then the line, flag and excluded values are synced, so the list's selection survives a box click.
Every action hands its raw value to one `CCue` gate.
  Free cue calls `CCueCreate`, and remove hands the ids of the list's selected rows to `CCueRemove`.
  The gate asks the confirm, so the driver asks nothing.
  The order buttons send the row's painted position, one up or one down, to `CCueMove`.
  The engine clamps the position.
  Either box hands both boxes of its row to `CCueSet`, so no painted value is sent back.
  The gate takes both boxes as bools, and Conduct converts the flag for the engine.
  The flag box takes `CCueRowFlag` as a bool, so the driver converts nothing.
The selection lives in the list control alone.
The driver keeps no state beyond the painted list.

## `QAccession`

Drives the adder `PAccession` from `CAccession`.
The search field hands its raw text to the aperture's `CApertureQuerySet`.
  Conduct then raises `CApertureRowsChanged`, which repaints the rows and the tally.
`QAccessionRefine` splices the rows from one `CAccessionRead`.
  The empty notice takes the key the aperture chose.
`QAccessionTallyRefine` paints the tally from the aperture in its own handler.
  So each refine sends one request.
A row shows its add button or its in-deck mark from the row's bound verdict.
  The add button hands the row's entry to `CAccessionAdd`.
  The add-all button calls `CAccessionShownAdd`, which skips bound rows in Conduct.

## `QAccessionItem`

One adder row, keyed by its entry and synced in place when its row record changes.

## `QJotter`

Drives the cell editor `PJotter` of one free flashcard, copying `QStencil`.
A listing selection, `CCueChanged` and `CDeckChanged` repaint it.
  It hands the selected cue ids to `CCueCellRead` as they stand, and a null answer collapses it.
  So Conduct alone decides one free cue, and the driver counts and tests nothing.
Each cell row is a `QJotterItem` holding the cue id and one `CCueCell`.
  The splice keeps the held rows while cue, column and index match, and only swaps their cell.
  So a body edit repaints in place, and the box keeps its focus and caret.
A body edit hands the row's cue, column and index and the box text to `CCueCellSet`.
  The handler is detached while a row paints, so painting sends no setter.
The pick button shows only on a cell Conduct marks as a file, and hands the same three to `CCueCellOpen`.

## `QJotterItem`

One painted cell row of the cell editor.
Its cell changes in place and notifies, so the row repaints.

## `QCueItem`

One painted row of the listing.
Its line, flag and excluded values change in place and notify, so the row repaints.
  A free cue's line follows its first text cell, so a cell edit keeps the row and its selection.

## Stamp

Commit: `2f9a06b8eb95`
- `QAccession.cs` `08d55b342988c03c`
- `QAccessionItem.cs` `40e6ca82c8ae2e65`
- `QCue.cs` `dc311de49858fdca`
- `QCueItem.cs` `c464340025c863c3`
- `QJotter.cs` `6f23f8819c563b58`
- `QJotterItem.cs` `4547fdba67f65e38`
