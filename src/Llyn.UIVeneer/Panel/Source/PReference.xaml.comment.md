# PReference.xaml.cs

## `public partial class PReference : UserControl`

The Source panel as a control: what it is made of, and what it forwards.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
Every branch it once carried lives in `LShelf` and the two `LPanel` it holds.
The panel news its deportment, forwards each handler to it, and writes its controls when a notice arrives.
The shelf, the entry list, the read areas, the two edit areas and the shared rail are all wired here.
The entry edit area is still a veneer hold, while the source edit area sits on the deportment's imprint.
The panel picks the one in front by the deportment's side verdict.

## `internal void PReferenceAttach(PWindow host)`

Puts the panel to work on the window deportment, which builds its deportment over the engine's ports.
It news the deportment with the seams the panel answers through and subscribes to the notices of both lists.
The editor's change notice goes to the deportment, which reads the imprint's desk itself.
It picks the one in front for the store button.
The editor's chronicle notice and the imprint desk's state notice refresh the rail's undo and redo.
The print and portrait command bindings are added last, so no can-execute query ever meets a deportment not yet built.

## `internal async void PReferenceVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Hands both vistas to the deportment and attaches the observers that carry each announcement onto the dispatcher.
A stored entry re-lists the shelf too, because the citation counts on its rows may have moved.
The flags are loaded before the first rows are built, because an entry row reads its flag at construction.

## `internal bool PReferenceChangeCheck()`

Whether either edit area holds modifications that have not been stored, as the deportment reads it.

## `internal bool PReferenceDraftFinish(bool store)`

Ends the draft of the area in front, committing it or discarding it.
The side verdict picks the editor's or the imprint's finish, and its answer is passed back up.

## `private bool PReferenceDiscardConfirm()`

The discard seam: the window's leave dialog over the finish of the area in front.

## `private bool PReferenceRemovalConfirm(int usage)`

The removal seam: the window's delete question, worded by how many rows still cite the Source.

## `private void PShelfUpdate()`

Refills the shelf from the rows the deportment reads, spliced so the list keeps its scroll position.
The citation chips of both source areas are rewritten, because a stored entry may have moved the count.

## `private void PFootnoteUpdate()`

Refills the entry list and picks its empty text by whether the list is being searched.

## `private void PReferenceModeUpdate()`

Writes which of the four areas is in front and the enablement the deportment holds into the rail.

## `private void PReferenceChronicleUpdate()`

Reads the chronicle of the area in front and writes the rail's undo and redo.

## `private void PReferencePressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the source being read, as the engine portrays it.
The window supplies both label sets, and the deportment picks the vista, so nothing is read back from the screen.

## Inline notes

### `PEditor.PEditorAttach(host, engine, "Reference", null);`

The editor opens on no entry: this panel puts it on one when the reader asks to write.
The origin names this panel, so its held work is told apart from the other panels'.

## `internal void PReferenceVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PReferenceRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PReferenceAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `internal long PReferenceVoyageRead()`

The Source the panel shows, read off the shelf panel as the station of this panel.

## `internal void PShelfSourceShow(long id)`

Shows one Source by id, the same selection a click on its shelf row makes.
The window's trail walks back into this panel through it.
