# QNotationItem.cs

## `internal sealed class QNotationItem`

One row of the pronunciation menu, standing for one source, built from the errand's ready row.
It carries the looked-up label, flag and notice text, and decides nothing about them.

## `internal QNotationItem(CNotationItem row)`

Looks up each reading's label and flag and the row's notice text.
A reading keeps its raw phonetic and variety, which a pick hands back to the errand.

## `public IReadOnlyList<QNotationReading> QNotationItemReading`

The readings this source gave so far, in the order they arrived, empty while none has.

## `public string QNotationItemNotice`

The one line a row without a reading says: searching, no entry, or failed to retrieve.

## `public bool QNotationItemReady`

Whether the row carries a reading the user can take now, which is what `CClipItemReady` means on an audio row.
It is what the template switches the readings and the notice on.

## `internal static void QNotationItemRefine(FrameworkElement container, object item, RoutedEventHandler select)`

Fills one source row: its name, its readings, and the italic line that stands in for them.
The readings show once the row is ready, and the line shows until then.
It attaches the reading fill to the row's own list, handing on the selection handler.
