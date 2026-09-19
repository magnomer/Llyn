# PRepertoireEntry.cs

## `public partial class PRepertoire`

The Entry side of the Repertoire panel.
`POccurrence` lists the entries referencing the chosen Situation, or every Entry while none is chosen.
A chosen row swaps the Situation reading for the panel's own entry display.
The mode toggle swaps that display for `PEditor`.
The rail's new and save answer here, since both branch on what stands in front.
The catalog and the Situation reading live in `PRepertoireBrowse.cs`.

## Inline notes

## `private void POccurrenceFind()`

Refills the middle column with the entries referencing the chosen Situation, or every Entry while none is chosen.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing references the Situation or nothing matches.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

### `private LVista? _pOccurrenceVista;`

The child vista whose chosen entry the right-hand side stands on, while it shows an Entry and not a Situation.
The right-hand side shows one or the other and never both.
An Entry row swaps the Situation reading for the entry display in place, and a Situation row swaps it back.
The chosen Situation keeps its mark meanwhile, because it still narrows the middle column.

## `private void POccurrenceEntryShow(long id)`

Reads one Entry back and shows it in the panel's own display, without leaving the tab.
An open Situation editor is cancelled first, after the caller has asked about its draft.
An editing side already open stays open, so the Entry lands in `PEditor` rather than in the display.
An Entry that is gone leaves the right-hand side on the Situation and refills the middle column.

## `private void POccurrenceEntryUpdate(LBulletin bulletin)`

An Entry stored while the editor holds a fresh one and shows nothing is that fresh one, and is adopted.
The catalog and its reference figures are then read again, since the store may reference a Situation.

## `private void PRepertoireEntryUpdate()`

Reached only for the entry the panel stands on, or for a store that named no entry.
The shown Entry is read back and redrawn.
A store that deleted it falls back to the chosen Situation, or clears the panel when none is chosen.

## `private void POccurrenceEntryCreate()`

Starts a fresh Entry in the entry editor, its first sense carrying the chosen Situation.
The Situation editor is put away first, since one draft at a time is held.
No Entry is shown yet, so the editor stands open with the row list unmarked until a store names one.
An Entry started with no Situation chosen comes up blank, since every Entry is listed then.

## `private void POccurrenceScribeHandle(bool editing)`

The mode toggle while an Entry is shown, mirroring the tenor panel.
Entering the editor starts a draft on the shown Entry, and leaving it asks first, then re-reads it.
`PRepertoireScribeHandle` routes here whenever an Entry rather than a Situation is shown, or a fresh one is being written.

## `private void POccurrenceScribeReset()`

Leaving the editor while a fresh Entry is being written, with no Entry shown yet.
The leave check may store the draft, and then the stored Entry is shown for reading.
Otherwise the draft is dropped and the panel goes back to the chosen Situation, or to nothing.

## `private void POccurrenceScribeShow(bool editing)`

Swaps the entry display for the entry editor and back.
The toggle marks follow, so the rail and the cell never disagree.
The undo and redo pair is settled last, since the editor now in front owns it.

## `private void PRepertoireStoreHandle(object sender, RoutedEventArgs e)`

Saves whichever editor is in front: the Entry through `PEditor`, or the Situation through `PScenarioStoreRun`.

## `private void PRepertoireFreshHandle(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list.
With no Situation chosen and no Entry shown, it opens the editor on a Situation nothing has stored yet.
With a Situation chosen, or an Entry shown, it starts a new Entry instead.

## `private void POccurrenceEntryHide()`

Puts the right-hand side back on the Situation side, on the same reading or editing side it stood on.
An open entry editor is reset first, so no entry draft outlives the Entry it was opened on.
The mode toggle comes back only while a Situation is chosen.
