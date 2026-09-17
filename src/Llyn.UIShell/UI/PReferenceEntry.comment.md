# PReferenceEntry.cs

## `public partial class PReference`

The Entry side of the Source panel.
`PFootnote` lists the entries citing the chosen Source, or every Entry while none is chosen.
A chosen row swaps the Source reading for the panel's own entry display.
The mode toggle swaps that display for `PEditor`.
The rail's new and save answer here, since both branch on what stands in front.
The shelf and the Source reading live in `PReferenceBrowse.cs`.

## Inline notes

## `private void PFootnoteFind()`

Refills the middle column with the entries citing the chosen Source, or every Entry while none is chosen.
A card cites by quoting an Example that names the Source.
So the row is the Entry the card belongs to.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing cites the Source or nothing matches.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

### `private long? _pDisplayEntry;`

The Entry the right-hand side stands on, held as an id, while it shows an Entry and not a Source.
The right-hand side shows one or the other and never both.
An Entry row swaps the Source reading for the entry display in place, and a Source row swaps it back.
The chosen Source keeps its mark meanwhile, because it still narrows the middle column.

## `private void PFootnoteEntryShow(long id)`

Reads one Entry back and shows it in the panel's own display, without leaving the tab.
An open Source editor is cancelled first, after the caller has asked about its draft.
An editing side already open stays open, so the Entry lands in `PEditor` rather than in the display.
An Entry that is gone leaves the right-hand side on the Source and refills the middle column.

## `private void PFootnoteEntryUpdate(long id)`

A store announced by the engine redraws the shown Entry when the store touched it.
A store that deleted it falls back to the chosen Source, or clears the panel when none is chosen.

## `private void PFootnoteEntryCreate()`

Starts a fresh Entry in the entry editor, its first sense citing the chosen Source.
The Source edit area is put away first, since one draft at a time is held.
No Entry is shown yet, so the editor stands open with the row list unmarked until a store names one.
An Entry started with no Source chosen comes up blank, since every Entry is listed then.

## `private void PFootnoteScribeHandle(bool editing)`

The mode toggle while an Entry is shown, mirroring the tenor panel.
Entering the editor starts a draft on the shown Entry, and leaving it asks first, then re-reads it.
`PReferenceScribeHandle` routes here whenever an Entry rather than a Source is shown, or a fresh one is being written.

## `private void PFootnoteScribeReset()`

Leaving the editor while a fresh Entry is being written, with no Entry shown yet.
The leave check may store the draft, and then the stored Entry is shown for reading.
Otherwise the draft is dropped and the panel goes back to the chosen Source, or to nothing.

## `private void PFootnoteScribeShow(bool editing)`

Swaps the entry display for the entry editor and back.
The toggle marks follow, so the rail and the cell never disagree.
The undo and redo pair is settled last, since the editor now in front owns it.

## `private void PReferenceChronicleUpdate()`

Lights `PReferenceBackward` and `PReferenceForward` from whichever editor is in front.
An Entry open in `PEditor` lends its chronicle state, and otherwise the source area's draft does.
The panel is the pair's only writer, so the two editors never overwrite each other.
Both editors' chronicle notices and the editor toggle call here.

## `private void PReferenceUndoHandle(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.
An Entry open in `PEditor` steps its own chronicle, and otherwise the edit area steps the held Source.

## `private void PReferenceRedoHandle(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.

## `private void PReferenceStoreHandle(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.
An Entry open in `PEditor` saves itself, and otherwise the edit area commits the held Source.

## `private void PFootnoteEntryHide()`

Puts the right-hand side back on the Source side, on the same reading or editing side it stood on.
An open entry editor is reset first, so no entry draft outlives the Entry it was opened on.
The mode toggle and the bin come back only while a Source is chosen.

## `private void PReferenceFreshHandle(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list.
With no Source chosen and no Entry shown, it opens the edit area on a Source nothing has stored yet.
With a Source chosen, or an Entry shown, it starts a new Entry instead.
The held work is started before the controls are filled, so the first keystroke already has somewhere to go.
