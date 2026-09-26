# LRepertoire.cs

## `public sealed class LRepertoire`

The repertoire panel's deportment: the situation list, the occurrence list, the entry editor and the scenario desk.
The situation list lives in the atlas, which holds the situation vista's panel state.
The occurrence list lives in the occurrence, whose rows follow the chosen Situation.
The desk holds the situation tenure the scenario editor edits, under the `Situation` scope.
The two vistas' chosen rows and editing flags are the panel's mode, and the controls only follow.
The occurrence list's loads and clears go straight to the editor's lectern, so the panel control relays no draft.

## `private LEditor LRepertoireEditor { get; }`

The entry editor's deportment on the occurrence side, which takes the occurrence vista when the panel's vistas are restored.

## `public LDesk LRepertoireDesk { get; }`

The desk over the situation draft, started by subject rather than by a vista.
The panel control attaches its observers to the tenure the desk announces, as the entry editor does.

## `public LAtlas LRepertoireAtlas { get; }`

The situation list, whose panel asks the desk whether the scenario holds unsaved changes.

## `public event Action? LRepertoireChanged`

Raised when the desk or the entry editor changes state, so the panel control refreshes its parts.

## `public event Action<LSituation?>? LRepertoireScenarioChanged`

Raised with the Situation the desk holds after a start, or null after a cancel or a refused start.

## `public event Action<LSituation>? LRepertoireSituationChanged`

Raised with the Situation a loaded draft carries, so the vignette shows it.

## `public event Action<string, Exception>? LRepertoireFailed`

Raised with a message key and the exception when a repertoire action fails.

## `public event Action? LRepertoireInquestCleared`

Raised once an arrival drops the inquest and the language filter, so the panel control empties both fields.

## `private void LRepertoireDraftStart(long? id)`

Starts the desk on a stored Situation or on nothing, from the `Repertoire` origin the recovered draft names.
It then announces the Situation the desk holds, which is null when the start was refused.

## `private LSituation? LRepertoireScenarioRead()`

Reads the Situation the desk holds, which persists the tenure first and so can throw.
A throw raises `LRepertoireFailed` with `Situation.HoldFailed` and reads as null, so no click handler sees it.

## `private void LRepertoireDraftOpen(long id)`

Starts the desk on the Situation the atlas panel opened for editing.

## `private void LRepertoireDraftCancel()`

Cancels the desk and announces that no scenario is held.

## `private void LRepertoireSituationUpdate(LDraft draft)`

Announces the Situation a loaded draft carries and ignores drafts of other subjects.

## `private void LRepertoireEditorOpen(long id)`

Opens the entry editor on the entry the occurrence panel opened for editing.

## `private void LRepertoireStateUpdate()`

Relays a desk or editor state change as `LRepertoireChanged`.

## `public LOccurrence LRepertoireOccurrence { get; }`

The occurrence list, whose panel asks the entry editor's desk whether the entry holds unsaved changes.

## `private bool LRepertoireOccurrenceSide`

The occurrence side is in front exactly when the occurrence list has a chosen row or is in edit mode.

## `public bool LRepertoireScenarioShown`

True while the situation side is in front and in edit mode, so the scenario shows.
The other mode flags follow the same two facts: which side is in front and whether it edits.
`LRepertoireVignetteShown` holds for the situation side outside edit mode.
`LRepertoireDisplayShown` holds for the occurrence side outside edit mode.
`LRepertoireEditorShown` holds while the occurrence list is in edit mode.
`LRepertoireScribeChecked` holds while either editor shows, and `LRepertoireViewerChecked` holds otherwise.
`LRepertoireModeEnabled` holds on the occurrence side, and on the situation side follows its panel.
`LRepertoireBinEnabled` holds only on the situation side, where it follows its panel.
Print takes an entry on display or a chosen Situation on the vignette.
Export takes only an entry on display.

## `public bool LRepertoireVignetteHeld`

True while the situation list has a chosen row, so the vignette body shows.

## `public bool LRepertoireVignetteBlank`

True while the situation list has no chosen row, so the unselected notice shows.

## `public bool LRepertoireStoreEnabled`

Follows the entry editor while it shows, and the scenario desk otherwise.

## `public bool LRepertoirePressAllowed`

Allows print for an entry on display or for a chosen Situation in the vignette.

## `public bool LRepertoirePortraitAllowed`

Allows export only for an entry on display.

## `public void LRepertoireSituationShow(long id)`

Shows a Situation that comes from outside the list, keeping the scribe open when either editor was open.
When the inquest or the filter hides it, the list clears it at once.
Both are then dropped and the Situation is shown again, so an arrival never lands on a blank vignette.
A row that vanished stays cleared, and without a inquest or filter nothing is dropped.

## `private void LRepertoireSituationShow(long id, bool editing)`

Clears the occurrence side and shows the Situation in the mode the caller read.
The atlas panel loads the row, clears a vanished one and restarts the scenario through its edit event.

## `public void LRepertoireSelect(long? id, Action record)`

Shows the clicked Situation once the user agrees to leave unsaved changes.
The mode is read before the question, because a save from the dialog must not drop the scribe.
The voyage is recorded only after the user agreed, so a refused leave keeps the Forward history.
A save from the dialog finishes through `LRepertoireDraftClose`, so the Situation loads once.

## `private void LRepertoireOccurrenceOpen(long id, bool editing)`

Loads the entry on the occurrence side first, and touches nothing else when it fails or has vanished.
Only a held entry cancels the scenario and closes the situation scribe.
The scribe then reopens on the entry when either editor was open.

## `public void LRepertoireOccurrenceSelect(long? id)`

Shows the chosen entry once the user agrees to leave unsaved changes.
The mode is read before the question, as `LRepertoireSelect` does.

## `public void LRepertoireFreshStart()`

Starts an occurrence for the chosen row when a row is chosen, else a blank Situation scenario.

## `public void LRepertoireScribeSet(bool editing)`

Toggles the scribe on the side in front.
Closing the occurrence side falls back to the chosen Situation, and closing the scenario cancels the desk.

## `private void LRepertoireSituationRestore(bool editing)`

Shows the chosen Situation again in the given mode, or clears both lists when none is chosen.

## `public void LRepertoireClear()`

Clears both lists, also when the workspace changes under the panel.

## `public void LRepertoireRowsApply(IReadOnlyList<LCatalogSituation> rows)`

Checks the rows the panel control has just applied, so a clear never re-enters a read in progress.
A chosen Situation missing from them clears both lists while the vignette shows it.

## `private bool LRepertoireRowHeld`

True while either list has a chosen row, so a fresh start opens an occurrence.

## `private bool LRepertoireRowShown`

True while the vignette shows a chosen Situation, the only case a missing row clears.

## `private void LRepertoireInquestClear()`

Drops the inquest and the language filter on the situation vista and raises `LRepertoireInquestCleared`.

## `public void LRepertoireSave()`

Saves the entry editor while it shows, else stores the scenario when it holds changes.
A stored scenario is shown again outside the scribe.

## `private void LRepertoireStoredShow(long id)`

Shows the stored Situation outside the scribe after the desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `public void LRepertoireDelete()`

Deletes the chosen Situation, and does nothing on the occurrence side.
A failure reads `Situation.DeleteFailed`, the delete key `LAtlas` hands its panel.

## `private void LRepertoireOccurrenceCreate()`

Opens a blank entry in the editor, attaching the chosen Situation when there is one.
The fresh open clears the occurrence panel, which closes the editor before the blank opens.

## `public bool LRepertoireChangeCheck()`

True when either list holds unsaved changes.

## `public bool LRepertoireLeaveConfirm()`

True when nothing is unsaved or the user agrees to leave.
A save from the dialog shows the stored Situation afterwards.

## `private bool LRepertoireLeaveConfirm(bool shown)`

Asks the leave seam only over unsaved changes, and hands it the finish a save runs.
The finish shows the stored Situation only when `shown` holds.

## `public void LRepertoireEntryUpdate()`

Refreshes the occurrence draft, and falls back to the chosen Situation once the occurrence side has closed.
It does nothing without a chosen occurrence, so a whole-set bulletin never clears a scenario or a new entry.
The mode is read before the refresh, so a vanished entry under edit reopens the scenario.

## `public bool LRepertoireDraftFinish(bool store)`

Finishes the entry editor while it shows, else the scenario desk.
A stored scenario is shown again outside the scribe.

## `private bool LRepertoireDraftClose(bool store)`

Finishes like `LRepertoireDraftFinish` but leaves a stored scenario unshown.
A row click uses it, because the click shows its own row next.

## `public (bool LDeskBackward, bool LDeskForward) LRepertoireChronicleRead()`

Reads undo and redo from the entry editor's desk while it shows, else from the scenario desk.

## `public void LRepertoireUndo()`

Steps the entry editor back while it shows, else the scenario desk.

## `public void LRepertoireRedo()`

Steps the entry editor forward while it shows, else the scenario desk.

## `private void LRepertoireChronicleRun(Action step)`

Runs a scenario step and raises `LRepertoireFailed` with `Situation.HoldFailed` when it throws.

## `public Task LRepertoirePortraitPrint(LPortraitLabel label, LPortraitLegend legend, LPressTicket ticket)`

Prints the entry on display, else the chosen Situation, else nothing.

## `public Task LRepertoirePortraitExport(string path, LPortraitMedium format, LPortraitLabel label)`

Exports the entry on display, and does nothing otherwise.
