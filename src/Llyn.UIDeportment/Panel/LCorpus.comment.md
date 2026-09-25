# LCorpus.cs

## `public sealed class LCorpus`

The corpus panel's deportment: the example list, the quotation list, the entry editor and the transcript desk.
The example list lives in the anthology, which holds the example vista's panel state.
The quotation list lives in the quotation, whose rows follow the chosen Example.
The desk holds the example tenure the transcript editor edits, under the `Example` scope.
The two vistas' chosen rows and editing flags are the panel's mode, and the controls only follow.

## `private LEditor LCorpusEditor { get; }`

The entry editor's deportment on the quotation side, which takes the quotation vista when the panel's vistas are restored.

## `public LDesk LCorpusDesk { get; }`

The desk over the example draft, started by subject rather than by a vista.
The veneer attaches its observers to the tenure the desk announces, as the entry editor does.

## `public LAnthology LCorpusAnthology { get; }`

The example list, whose panel asks the desk whether the transcript holds unsaved changes.

## `public event Action? LCorpusChanged`

Raised when the desk or the entry editor changes state, so the veneer refreshes the panel.

## `public event Action<LExample?>? LCorpusTranscriptChanged`

Raised with the Example the desk holds after a start, or null after a cancel or a refused start.

## `public event Action<LExample>? LCorpusExampleChanged`

Raised with the Example a loaded draft carries, so the excerpt shows it.

## `public event Action<string, Exception>? LCorpusFailed`

Raised with a message key and the exception when a corpus action fails.

## `public event Action? LCorpusQueryCleared`

Raised once an arrival drops the query and the language filter, so the veneer empties both controls.

## `private void LCorpusDraftStart(long? id)`

Starts the desk on a stored Example or on nothing, from the `Corpus` origin the recovered draft names.
It then announces the Example the desk holds, which is null when the start was refused.

## `private LExample? LCorpusTranscriptRead()`

Reads the Example the desk holds, which persists the tenure first and so can throw.
A throw raises `LCorpusFailed` with `Example.HoldFailed` and reads as null, so no click handler sees it.

## `private void LCorpusDraftOpen(long id)`

Starts the desk on the Example the anthology panel opened for editing.

## `private void LCorpusDraftCancel()`

Cancels the desk and announces that no transcript is held.

## `private void LCorpusExampleUpdate(LDraft draft)`

Announces the Example a loaded draft carries and ignores drafts of other subjects.

## `private void LCorpusEditorOpen(long id)`

Opens the entry editor on the entry the quotation panel opened for editing.

## `private void LCorpusStateUpdate()`

Relays a desk or editor state change as `LCorpusChanged`.

## `public LQuotation LCorpusQuotation { get; }`

The quotation list, whose panel asks the entry editor's desk whether the entry holds unsaved changes.

## `private bool LCorpusQuotationSide`

The quotation side is in front exactly when the quotation list has a chosen row or is in edit mode.

## `public bool LCorpusTranscriptShown`

True while the example side is in front and in edit mode, so the transcript shows.
The other mode flags follow the same two facts: which side is in front and whether it edits.
`LCorpusExcerptShown` holds for the example side outside edit mode.
`LCorpusDisplayShown` holds for the quotation side outside edit mode.
`LCorpusEditorShown` holds while the quotation list is in edit mode.
`LCorpusScribeChecked` holds while either editor shows, and `LCorpusViewerChecked` holds otherwise.
`LCorpusModeEnabled` holds on the quotation side, and on the example side follows its panel.
`LCorpusBinEnabled` holds only on the example side, where it follows its panel.
Print takes an entry on display or a chosen Example on the excerpt.
Export takes only an entry on display.

## `public bool LCorpusExcerptHeld`

True while the example list has a chosen row, so the excerpt body shows.

## `public bool LCorpusExcerptBlank`

True while the example list has no chosen row, so the unselected notice shows.

## `public bool LCorpusStoreEnabled`

Follows the entry editor while it shows, and the transcript desk otherwise.

## `public bool LCorpusPressAllowed`

Allows print for an entry on display or for a chosen example in the excerpt.

## `public bool LCorpusPortraitAllowed`

Allows export only for an entry on display.

## `public void LCorpusExampleShow(long id)`

Shows an Example that comes from outside the list, keeping the scribe open when either editor was open.
When the query or the filter hides it, the list clears it at once.
Both are then dropped and the Example is shown again, so an arrival never lands on a blank excerpt.
A row that vanished stays cleared, and without a query or filter nothing is dropped.

## `private void LCorpusExampleShow(long id, bool editing)`

Clears the quotation side and shows the Example in the mode the caller read.
The anthology panel loads the row, clears a vanished one and restarts the transcript through its edit event.

## `public void LCorpusSelect(long? id, Action record)`

Shows the clicked Example once the user agrees to leave unsaved changes.
The mode is read before the question, because a save from the dialog must not drop the scribe.
The voyage is recorded only after the user agreed, so a refused leave keeps the Forward history.
A save from the dialog finishes through `LCorpusDraftClose`, so the Example loads once.

## `private void LCorpusQuotationOpen(long id, bool editing)`

Loads the entry on the quotation side first, and touches nothing else when it fails or has vanished.
Only a held entry cancels the transcript and closes the example scribe.
The scribe then reopens on the entry when either editor was open.

## `public void LCorpusQuotationSelect(long? id)`

Shows the chosen entry once the user agrees to leave unsaved changes.
The mode is read before the question, as `LCorpusSelect` does.

## `public void LCorpusFreshStart()`

Starts a quotation for the chosen row when a row is chosen, else a blank Example transcript.

## `public void LCorpusScribeSet(bool editing)`

Toggles the scribe on the side in front.
Closing the quotation side falls back to the chosen Example, and closing the transcript cancels the desk.

## `private void LCorpusExampleRestore(bool editing)`

Shows the chosen Example again in the given mode, or clears both lists when none is chosen.

## `public void LCorpusClear()`

Clears both lists, also when the workspace changes under the panel.

## `public void LCorpusRowsApply(IReadOnlyList<LCatalogExample> rows)`

Checks the rows the veneer has just applied, so a clear never re-enters a read in progress.
A chosen Example missing from them clears both lists while the excerpt shows it.

## `private bool LCorpusRowHeld`

True while either list has a chosen row, so a fresh start opens a quotation.

## `private bool LCorpusRowShown`

True while the excerpt shows a chosen Example, the only case a missing row clears.

## `private void LCorpusQueryClear()`

Drops the query and the language filter on the example vista and raises `LCorpusQueryCleared`.

## `public void LCorpusSave()`

Saves the entry editor while it shows, else stores the transcript when it holds changes.
A stored transcript is shown again outside the scribe.

## `private void LCorpusStoredShow(long id)`

Shows the stored Example outside the scribe after the desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `public void LCorpusDelete()`

Deletes the chosen Example, and does nothing on the quotation side.
A failure reads `Example.DeleteFailed`, the delete key `LAnthology` hands its panel.

## `private void LCorpusQuotationCreate()`

Opens a blank entry in the editor, attaching the chosen Example when there is one.
The fresh open clears the quotation panel, which closes the editor before the blank opens.

## `public bool LCorpusChangeCheck()`

True when either list holds unsaved changes.

## `public bool LCorpusLeaveConfirm()`

True when nothing is unsaved or the user agrees to leave.
A save from the dialog shows the stored Example afterwards.

## `private bool LCorpusLeaveConfirm(bool shown)`

Asks the leave seam only over unsaved changes, and hands it the finish a save runs.
The finish shows the stored Example only when `shown` holds.

## `public void LCorpusEntryUpdate()`

Refreshes the quotation draft, and falls back to the chosen Example once the quotation side has closed.
It does nothing without a chosen quotation, so a whole-set bulletin never clears a transcript or a new entry.
The mode is read before the refresh, so a vanished entry under edit reopens the transcript.

## `public bool LCorpusDraftFinish(bool store)`

Finishes the entry editor while it shows, else the transcript desk.
A stored transcript is shown again outside the scribe.

## `private bool LCorpusDraftClose(bool store)`

Finishes like `LCorpusDraftFinish` but leaves a stored transcript unshown.
A row click uses it, because the click shows its own row next.

## `public (bool LDeskBackward, bool LDeskForward) LCorpusChronicleRead()`

Reads undo and redo from the entry editor's desk while it shows, else from the transcript desk.

## `public void LCorpusUndo()`

Steps the entry editor back while it shows, else the transcript desk.

## `public void LCorpusRedo()`

Steps the entry editor forward while it shows, else the transcript desk.

## `private void LCorpusChronicleRun(Action step)`

Runs a transcript step and raises `LCorpusFailed` with `Example.HoldFailed` when it throws.

## `public Task LCorpusPortraitPrint(LPortraitLabel label, LPortraitLegend legend, LPressTicket ticket)`

Prints the entry on display, else the chosen Example, else nothing.

## `public Task LCorpusPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)`

Exports the entry on display, and does nothing otherwise.
