# CCorpus.cs

## `public sealed class CCorpus`

The corpus panel's session: the example list, the quotation list, the transcript desk and the entry editor.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a panel, the desk or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints, exports or deletes.

## `private CCorpus(CAtelier atelier, CEditor editor, Func<bool> shownSeam, CEnvoy envoy)`

Builds the desk under the `Example` scope, the anthology over it and the quotation panel over the editor's desk.
The session defers to the entry editor while the quotation list edits.
A stored Example is shown again outside the scribe through `LCorpusStoredShow`.
The quotation panel opens the editor on the entry it edits, and a clear cancels the editor's desk.
The anthology's row notices reach the quotation panel, whose rows follow the chosen Example.

## `public static CCorpus CCorpusCreate(CAtelier atelier, CEditor editor, Func<bool> shownSeam, CEnvoy envoy)`

Builds the corpus over the atelier and the panel's own editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public event Action? CCorpusChanged`

Raised when the desk or the entry editor changes state, so a driver repaints the mode.

## `public event Action<CExample?>? CCorpusTranscriptChanged`

Raised with the Example the desk holds after a start, or null after a cancel or a refused start.

## `public event Action<CExample>? CCorpusExampleChanged`

Raised with the Example a loaded anthology draft carries, so the excerpt shows it.

## `public event Action? CCorpusQueryCleared`

Raised once an arrival drops the query and the language filter, so a driver empties both controls.

## `private readonly CAtelier _cCorpusAtelier`

The atelier the two vistas start through when the forge restores them.

## `public CQuotation CCorpusQuotation { get; }`

The quotation list, whose panel asks the entry editor's desk whether the entry holds unsaved changes.
It is built over the atelier's entry and portrait ports, so no driver holds either.

## `public bool CCorpusTranscriptShown`

True while the example side is in front and in edit mode, so the transcript shows.
The other mode flags follow the same two facts: which side is in front and whether it edits.
`CCorpusExcerptShown` holds for the example side outside edit mode.
`CCorpusDisplayShown` holds for the quotation side outside edit mode.
`CCorpusEditorShown` holds while the quotation list is in edit mode.
`CCorpusScribeChecked` holds while either editor shows, and `CCorpusViewerChecked` holds otherwise.
`CCorpusModeEnabled` holds on the quotation side, and on the example side follows its panel.
`CCorpusBinEnabled` holds only on the example side, where it follows its panel.
`CCorpusExcerptHeld` and `CCorpusExcerptBlank` say whether the example list has a chosen row.
`CCorpusStoreEnabled` follows the entry editor while it shows, and the transcript desk otherwise.

## `public bool CCorpusPressAllowed`

Print takes an entry on display or a chosen Example on the excerpt.
Export, as `CCorpusPortraitAllowed` says, takes only an entry on display.

## `private bool LCorpusQuotationSide`

The quotation side is in front exactly when the quotation list has a chosen row or is in edit mode.
No driver reads it, since the mode flags it feeds answer every question a driver asks.

## `private bool LCorpusRowShown`

True while the excerpt shows a chosen Example, the only case a missing row clears.

## `private bool LCorpusRowHeld`

True while either list has a chosen row, so a fresh start opens a quotation.

## `public CExample? CCorpusTranscriptRead()`

Reads the Example the desk holds, which persists the tenure first and so can throw.
A throw reports `Example.HoldFailed` through the envoy and reads as null.
A driver reads it on each draft bulletin, so the desk never hands it a draft.

## `private void LCorpusExampleUpdate(CExample? example)`

Announces the Example a loaded draft carries and ignores drafts of other subjects.

## `public void CCorpusExampleOpen(long id)`

Shows an Example that comes from outside the list, keeping the scribe open when either editor was open.
When the query or the filter hides it, the list clears it at once.
Both are then dropped and the Example is shown again, so an arrival never lands on a blank excerpt.
A row that vanished stays cleared, and without a query or filter nothing is dropped.

## `private void LCorpusExampleShow(long id, bool editing)`

Clears the quotation side and shows the Example in the mode the caller read.
The anthology panel loads the row, clears a vanished one and restarts the transcript through its edit event.

## `public void CCorpusExampleSelect(long? id, Action record)`

Shows the clicked Example once the user agrees to leave unsaved changes.
The mode is read before the question, because a save from the dialog must not drop the scribe.
The voyage is recorded only after the user agreed, so a refused leave keeps the Forward history.
A save from the dialog finishes through `CSessionClose`, so the Example loads once.

## `private void LCorpusQuotationOpen(long id, bool editing)`

Loads the entry on the quotation side first, and touches nothing else when it fails or has vanished.
Only a held entry cancels the transcript and closes the example scribe.
The scribe then reopens on the entry when either editor was open.

## `public void CCorpusQuotationSelect(long? id)`

Shows the chosen entry once the user agrees to leave unsaved changes.
The mode is read before the question, as `CCorpusExampleSelect` does.

## `public void CCorpusExampleCreate()`

Starts a quotation for the chosen row when a row is chosen, else a blank Example transcript.

## `public void CCorpusScribeToggle(bool editing)`

Toggles the scribe on the side in front.
Closing the quotation side falls back to the chosen Example, and closing the transcript cancels the desk.

## `private void LCorpusExampleRestore(bool editing)`

Shows the chosen Example again in the given mode, or clears both lists when none is chosen.

## `public void CCorpusExampleClose()`

Clears both lists, also when the workspace changes under the panel.

## `private void LCorpusQueryClear()`

Drops the query and the language filter on the example vista and raises `CCorpusQueryCleared`.

## `private void LCorpusStoredShow(long id)`

Shows the stored Example outside the scribe after the desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `private void LCorpusQuotationCreate()`

Opens a fresh entry in the editor, already citing the chosen Example when there is one.
The engine starts and cites it in one call, so the first paint shows the citation.
The fresh open clears the quotation panel, which closes the editor before the blank opens.

## `public bool CCorpusLeaveConfirm()`

True when nothing is unsaved or the user agrees to leave.
A save from the dialog shows the stored Example afterwards.

## `private bool LCorpusLeaveConfirm(bool shown)`

Asks the envoy only over unsaved changes.
A store finishes the session, and a discard leaves without touching it.
The finish shows the stored Example only when `shown` holds.

## `public void CCorpusEntryResonate()`

Answers the chosen entry's notice by refreshing the quotation draft.
It falls back to the chosen Example once the quotation side has closed.
It does nothing without a chosen quotation, so a whole-set bulletin never clears a transcript or a new entry.
The mode is read before the refresh, so a vanished entry under edit reopens the transcript.

## `public IReadOnlyList<CCatalogExample> CCorpusRowsRead(string unknown, string unwritten)`

Reads the example list's rows, worded by the driver's two placeholders.
When the excerpt shows a chosen Example the rows no longer list, both lists clear before the rows return.
The clear announces fresh rows, which carry the same Examples, so a driver's nested refill is harmless.

## `public void CCorpusExampleDelete()`

Deletes the chosen Example through the anthology panel, which asks the envoy first.
It does nothing on the quotation side, whose panel has no delete scope.
A failure reads `Example.DeleteFailed`, the delete key `CAnthology` hands its panel.

## `public Task CCorpusPortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)`

Prints the entry on display, else the chosen Example on the excerpt, else nothing.
The label serves the entry and the legend serves the Example list.

## `public Task CCorpusPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)`

Exports the entry on display, and does nothing otherwise.

## `public void CCorpusVistaRestore()`

Starts the `corpus` vista over Examples by text and the `quotation` vista over entries by headword.
The anthology takes the first, and the quotation list takes both, since its rows follow the chosen Example.
The entry editor takes the quotation vista, so an entry it stores lands in that list.
The forge runs it at build and again after a workspace change.
