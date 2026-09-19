# PReference.xaml.cs

## `public partial class PReference : UserControl`

The Source panel as a control: what it is made of, and what it forwards.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
Every branch it once carried lives in `LShelf` and the two `LPanel` it holds.
The panel news its deportment, forwards each handler to it, and writes its controls when a notice arrives.
The shelf, the entry list, the read areas, the two edit areas and the shared rail are all wired here.
The two edit areas are still veneer holds.
The panel picks the one in front by the deportment's side verdict.

## `internal void PReferenceAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened.
It news the deportment with the seams the panel answers through and subscribes to the notices of both lists.
The editor's and the imprint's change notices go to the deportment.
It picks the one in front for the store button.
Both areas' chronicle notices refresh the rail's undo and redo through the same side verdict.

## `internal async void PReferenceVistaRestore(LVista vista, LVista footnote)`

Hands both vistas to the deportment and attaches the observers that carry each announcement onto the dispatcher.
A stored entry re-lists the shelf too, because the citation counts on its rows may have moved.
The flags are loaded before the first rows are built, because an entry row reads its flag at construction.

## `internal bool PReferenceChangeCheck()`

Whether either edit area holds modifications that have not been stored, as the deportment reads it.

## `internal bool PReferenceDraftFinish(bool store)`

Ends the draft of the area in front, committing it or discarding it.
The side verdict picks the editor's or the imprint's finish, and its answer is passed back up.

## `internal string PReferenceTallyRead(long? id)`

The citation sentence for one Source, which the imprint asks for the draft it holds.

## `internal void PReferenceShow(long id)`

Puts the source side on one stored Source in its read area, for the imprint after a store.

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
