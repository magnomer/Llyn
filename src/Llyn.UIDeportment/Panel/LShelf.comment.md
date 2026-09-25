# LShelf.cs

## `public sealed class LShelf`

The deportment of the sources panel: two panel states, one over the shelf and one over the citing entries.
The base is Shelf rather than Reference because the Source record already holds that logic name.
The tab stands on one side at a time.
The entry side is in front while an entry is chosen or being written, else the source side.
Each side has a read area and an edit area, and four verdicts say which one is in front.
The source edit area is the imprint deportment held here, so its draft answers are read directly.
The entry edit area is still a veneer hold, so its change answer arrives through a seam.
The unreadable seam is the window's sweep question, passed on to the imprint's desk.
The leave seam is the window's discard dialog, asked once for the whole tab before any switch of side.
The removal seam is the window's delete question, worded by how many rows still cite the Source.
The citing entries' loads and clears go straight to the editor's lectern, so the veneer relays no draft.

## `public event Action? LShelfChanged;`

A store flag moved, so the store button is read again.

## `public LEditor LShelfEditor { get; }`

The entry editor's deportment on the entry side.
Its state feeds the rail as the imprint's desk does on the source side.

## `public (bool LShelfBackward, bool LShelfForward) LShelfChronicleRead()`

The chronicle of the side in front, so the rail's undo and redo need no pick of their own.

## `public bool LShelfDraftFinish(bool store)`

The window's leave over the side in front.

## `public void LShelfStoreRun()`

The rail's save over the side in front.

## `public LImprint LShelfImprint { get; }`

The source editor's deportment, opened by an edited row or a fresh start and dropped when the area leaves.
Its stored draft is shown again through the panel state, as the stored Source.

## `public bool LShelfEntrySide`

The entry side is in front exactly when the entry list has a chosen row or is in edit mode.

## `private IReadOnlyList<LCatalogReference> LShelfRowsApply(IReadOnlyList<LCatalogReference> rows)`

Counts the rows and drops a chosen Source the search no longer lists, unless it is being edited.
The rows cross as a parameter, so no local carries an engine answer into the clear.

## `public string LShelfTallyRead(long? id)`

The citation sentence for one Source, composed from the usage the engine counts.
The page asks it by the chosen row.

## `public LColophon LShelfColophonRead(LDraft draft)`

The read sheet of a loaded Source draft with the tally of the chosen row.
The draft arrives as a parameter from the notice, so the Source is never a local here.

## `public void LShelfRowSelect(long? id)`

A click on the shelf: asks once for the tab, then opens the Source on the source side.
The mode carries over, so a Source clicked while writing opens in the imprint.

## `public void LShelfRowShow(long id)`

A jump from another panel: opens the Source without asking, since the window has already asked.

## `private void LShelfSourceOpen(long? id, bool editing)`

Clears the entry list, sets the carried mode, then loads the Source.
The mode crosses as a parameter, because reading it after the clear would read the cleared list.

## `public void LShelfEntrySelect(long? id)`

A click on the entry list: asks once for the tab, closes the source edit, then opens the entry.
The mode carries over, so an entry clicked while writing opens in the editor.

## `public void LShelfEntryUpdate()`

The chosen entry changed under the panel: reloaded quietly, and a vanished one returns to the source side.

## `public void LShelfFreshStart()`

With a Source or an entry chosen, starts an entry under the chosen Source.
With nothing chosen, starts a Source draft in the imprint.

## `public void LShelfScribeSet(bool editing)`

The mode toggle of the side in front.
Leaving a fresh entry with nothing chosen returns to the chosen Source in its read area.
Leaving the imprint drops its held draft after the panel state has reloaded the Source.

## `private void LShelfImprintOpen(long id)`

The panel's edited notice, carried to the imprint as a nullable id.

## `public void LShelfDelete()`

Deletes the chosen Source from the source side alone, behind the removal seam.

## `public Task LShelfPortraitPrint(LPortraitLabel label, LPortraitLegend legend, LPressTicket ticket)`

Prints the entry being read, else the Source being read, as the engine portrays it.
Both label sets arrive because the deportment, not the window, knows which side prints.
