# PCorpusBrowse.cs

## `public partial class PCorpus`

Browsing behavior of the Corpus panel.
`PAnthology` lists every Example the workspace holds, including one nothing quotes.
A chosen row is read back and shown, and the middle column narrows to the entries quoting it.
`PCorpusScribe` swaps the reading for the editor.
The panel answers two questions rather than one: what this sentence is, and where it is quoted.

## Inline notes

### `private IReadOnlyDictionary<string, int> _pAnthologyCount = new Dictionary<string, int>();`

How many places quote each Example, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

## `private async void PCorpusBulletinHandle(LBulletin bulletin)`

Re-reads the workspace whenever the engine announces a change, wherever it was made.
A draft bulletin is the panel's own typing coming back, so it redraws the editor and reads nothing else.
The citations are read with the catalog, because a Source stored elsewhere is what a row cites.
A workspace that moved reloads the flags and the language menu first, since neither belongs to the old folder's rows.
An Entry stored while the editor holds a fresh one and shows nothing is that fresh one, and is adopted.
Only an Entry announcement is read that way, since a frequency or tag announcement carries another id.

## `private void PAnthologyFind(string query)`

Refills the catalog with the rows the engine returns, already matched and already ordered.
The panel names the ordering and the query and decides nothing else about either.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.
Rows sharing a sentence are numbered afterwards, so the reader can tell them apart.
Both tally chips are rewritten from the fresh counts, so a quotation added elsewhere shows at once.

## `internal void PAnthologyExampleShow(long id)`

Reads one Example back and shows it with the sides quoting it.
An open editor is restarted on the Example chosen, so what it writes into is the draft for that sentence.
An id the store no longer knows clears the selection and refills the catalog rather than failing.
The window calls it so a Source citing row can land on the Example it names.

## `private void PCorpusBinHandle(object sender, RoutedEventArgs e)`

Deletes the Example the panel stands on, from the reading side or the editing side alike.
It answers only while an Example and not an Entry is shown, because an Entry is deleted from the library.
An Example nothing quotes is simply deleted.
One something quotes is named to the user first, and only then detached and deleted in one operation.

## `private string? PExcerptTextRead(LStateValue value)`

The text a three-state value reads as, or null when it was never written.
An unknown value reads the unknown mark, as the situation reading reads it.

## `private void PExcerptSentenceShow(LExample example)`

Writes the sentence at the head of the page, as a situation's title stands at the head of its page.
A never-written sentence reads the unwritten text in the muted colour, because the head of the page cannot be empty.
The sentence field is the clickable control, and it takes its text through its own property.
Its Mentions are handed over only when the text shown is the stored sentence.
The unknown mark and the unwritten text have no words for a Mention to lie on.

## `private void PExcerptCitationShow(LStateAnchor value)`

Writes the cited Source under its heading, or hides the heading when no citation was ever written.
An unknown citation reads the unknown mark, so the two kinds of empty stay distinct.

## `private string PCorpusTallyRead(long? id)`

The usage count as the tally chip reads it, a sentence rather than a bare number.
It reads the same count the delete confirmation reads, so the chip and the warning cannot disagree.

## `private void PExcerptMentionHandle(object? sender, PMentionArgument e)`

A click on a word of the open Example asks the engine what stands at that offset.
The engine reads the stored sentence itself, so the panel passes the id and the offset and nothing more.
The window decides what the answer opens, as it does for the display cards.
An open draft is confirmed first, because landing on an Entry leaves the corpus panel.

## `private void PQuotationFind()`

Refills the middle column with the entries quoting the chosen Example, or every Entry while none is chosen.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing quotes the Example or nothing matches.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

### `private long? _pDisplayEntry;`

The Entry the right-hand side stands on, held as an id, while it shows an Entry and not an Example.
The right-hand side shows one or the other and never both.
An Entry row swaps the Example reading for the entry display in place, and an Example row swaps it back.
The chosen Example keeps its mark meanwhile, because it still narrows the middle column.

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

## `private void PCorpusStoreHandle(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.
An Entry open in `PEditor` saves itself, and otherwise the held Example is committed.

## `private void PQuotationEntryHide()`

Puts the right-hand side back on the Example side, on the same reading or editing side it stood on.
An open entry editor is reset first, so no entry draft outlives the Entry it was opened on.
The mode toggle and the bin come back only while an Example is chosen.

## `private void PCorpusScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the editor and back.
Entering the editor starts a draft on the selected Example, and leaving it discards that draft.
Leaving the editor asks first, so unsaved wording is never lost silently.


## `if (editing == (PTranscript.Visibility == Visibility.Visible))`

Both segments answer here, so the click is read off which one was pressed.
The segment already standing for what is on screen changes nothing.
## `private void PCorpusClear()`

Drops the selection and empties the reading, the usage and the editor together.
The held draft goes first, because nothing may write into a draft the panel no longer stands on.
The mode toggle and the bin are disabled with it.
There is nothing to edit or delete while nothing is selected.

### `internal void PRankRestore(LCatalogOrder order)`

Puts the panel back on the ordering the settings stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PGauzeRestore(LCatalogFilter filter)`

Puts the language filter back on the languages the settings stored, and builds the menu over the loaded languages.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PCorpusScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PAnthologySelect(string? id)`

Marks the catalog row the panel stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.
It is called wherever the shown example changes, so the mark and the right-hand side never disagree.
