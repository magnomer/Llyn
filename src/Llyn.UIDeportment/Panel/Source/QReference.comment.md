# QReference.cs

## `internal sealed class QReference`

The driver of the Source panel: what it is made of, and what it forwards.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
Every branch it once carried lives in `LShelf` and the two `LPanel` it holds.
The driver forwards each handler to the shelf and writes the page's parts when a notice arrives.
The shelf, the entry list, the read areas, the two edit areas and the shared rail are all wired here.
The driver picks the area in front by the shelf's side verdict.

## `internal QReference(UserControl surface)`

Takes the veneer page as its surface and builds the drivers of the nested imprint and colophon pages.
It points the export and print buttons at their commands, ties both droppers, and sets every icon.
It subscribes every click, both search fields and both list clicks, and attaches the two item fills.

## `private Border QGrade`

Each named part of the page is found through `QContract.QContractFind` under its markup name.

## `internal void QReferenceAttach(PWindow host)`

Puts the driver to work on the window deportment, which builds the shelf over the engine's ports.
It hands the shelf the seams the driver answers through and subscribes to the notices of both lists.
The editor's chronicle notice and the imprint desk's state notice refresh the rail's undo and redo.
The print and portrait command bindings are added last, so no can-execute query meets a shelf not yet built.

## `internal async void QReferenceVistaRestore()`

Attaches the observers that carry each announcement onto the dispatcher.
A stored entry re-lists the shelf too, because the citation counts on its rows may have moved.
The offered orders come from `LShelf.LShelfOrderRead`, so the driver lists no order of its own.
The flags are loaded before the first rows are built, because an entry row reads its flag at construction.

## `internal bool QReferenceChangeCheck()`

Whether either edit area holds modifications that have not been stored, as the shelf reads it.

## `internal bool QReferenceDraftFinish(bool store)`

Ends the draft of the area in front, committing it or discarding it.
The side verdict picks the editor's or the imprint's finish, and its answer is passed back up.

## `internal bool QReferenceLeaveConfirm()`

The panel's question before its unsaved work goes out of sight, asked by the window.

## `private bool QReferenceDiscardConfirm()`

The discard seam: the window's leave dialog over the finish of the area in front.

## `private bool QReferenceRemovalConfirm(int usage)`

The removal seam: the window's delete question, worded by how many rows still cite the Source.

## `private void QShelfUpdate()`

Refills the shelf from the rows the shelf reads, spliced so the list keeps its scroll position.
The citation chips of both source areas are rewritten, because a stored entry may have moved the count.

## `private void QFootnoteUpdate()`

Refills the entry list and picks its empty text by whether the list is being searched.

## `private void QReferenceModeUpdate()`

Writes which of the four areas is in front and the enablement the shelf holds into the rail.
The voyage and chronicle pairs follow the viewer and scribe verdicts.

## `private void QReferenceChronicleUpdate()`

Reads the chronicle of the area in front and writes the rail's undo and redo.

## `private async void QReferencePressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the source being read, as the engine portrays it.
The window supplies the label and the Source legend as Conduct shapes, and the shelf picks the vista.
Nothing is read back from the screen.

## `internal void QReferenceVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the driver only shows what it is told.

## `private void QReferenceRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void QReferenceAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `internal long QReferenceVoyageRead()`

The Source the panel shows, read off the shelf panel as the station of this panel.

## `internal void QShelfSourceShow(long id)`

Shows one Source by id, for a jump the window makes from another panel.
It asks nothing, because the window asks before it jumps.
