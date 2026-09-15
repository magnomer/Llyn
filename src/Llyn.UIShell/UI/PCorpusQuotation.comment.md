# PCorpusQuotation.cs

## `public partial class PCorpus`

The middle column of the Corpus panel, and the entry display it opens on the right.
`PQuotation` lists the entries quoting the chosen Example, or every Entry while none is chosen.
A chosen row swaps the Example reading for the entry display in place, without leaving the tab.

## Inline notes

### `private long? _pDisplayEntry;`

The Entry the right-hand side stands on, held as an id, while it shows an Entry and not an Example.
The right-hand side shows one or the other and never both.
An Entry row swaps the Example reading for the entry display in place, and an Example row swaps it back.
The chosen Example keeps its mark meanwhile, because it still narrows the middle column.

## `private void PQuotationFind()`

Refills the middle column with the entries quoting the chosen Example, or every Entry while none is chosen.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing quotes the Example or nothing matches.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

## `private void PQuotationEntryShow(long id)`

Reads one Entry back and shows it in the panel's own display, without leaving the tab.
An open Example editor is cancelled first, after the caller has asked about its draft.
An editing side already open stays open, so the Entry lands in `PEditor` rather than in the display.
An Entry that is gone leaves the right-hand side on the Example and refills the middle column.

## `private void PQuotationEntryCreate()`

Starts a fresh Entry in the entry editor, its first sense quoting the chosen Example.
The sentence editor is put away first, since one draft at a time is held.
No Entry is shown yet, so the editor stands open with the row list unmarked until a store names one.
An Entry started with no Example chosen comes up blank, since every Entry is listed then.

## `private void PQuotationEntryUpdate(long id)`

A store announced by the engine redraws the shown Entry when the store touched it.
A store that deleted it falls back to the chosen Example, or clears the panel when none is chosen.

## `private void PQuotationScribeHandle(bool editing)`

The mode toggle while an Entry is shown, mirroring the tenor panel.
Entering the editor starts a draft on the shown Entry, and leaving it asks first, then re-reads it.
`PCorpusScribeHandle` routes here whenever an Entry rather than an Example is shown, or a fresh one is being written.

## `private void PQuotationScribeReset()`

Leaving the editor while a fresh Entry is being written, with no Entry shown yet.
The leave check may store the draft, and then the stored Entry is shown for reading.
Otherwise the draft is dropped and the panel goes back to the chosen Example, or to nothing.

## `private void PQuotationScribeShow(bool editing)`

Swaps the entry display for the entry editor and back.
The toggle marks follow, so the rail and the cell never disagree.

## `private void PQuotationEntryHide()`

Puts the right-hand side back on the Example side, on the same reading or editing side it stood on.
An open entry editor is reset first, so no entry draft outlives the Entry it was opened on.
The mode toggle and the bin come back only while an Example is chosen.
