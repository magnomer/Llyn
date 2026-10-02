# CRepertoire.cs
Hash: `e5fb55b1c7ad8b81`

## `public sealed class CRepertoire`

The repertoire panel's session: the situation list, the occurrence list, the scenario desk and the entry editor.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a panel, the desk or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints, exports or deletes.

## `private CRepertoire(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the desk under the `Situation` scope, the atlas over it and the occurrence list over the editor's desk.
The desk starts by subject rather than by a vista, with the `Repertoire` origin.
The session defers to the entry editor while the occurrence list edits.
A stored Situation is shown again outside the scribe through `LRepertoireStoredShow`.
The occurrence panel opens the editor on the entry it edits, and a clear cancels the editor's desk.
The atlas's row notices reach the occurrence panel, whose rows follow the chosen Situation.
It registers `LRepertoireClose` with the workspace and attaches the scenario desk's observers through the marshal.
It keeps the marshal, so each vista restore attaches the lists' observers on the fresh vistas.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CRepertoire CRepertoireCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the repertoire over the atelier and the panel's own editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public event Action? CRepertoireChanged`

Raised when the desk or the entry editor changes state, so a driver repaints the mode.

## `public event Action<CScenario>? CRepertoireScenarioChanged`

Raised with the scenario of the Situation the desk holds after a start.
A cancel or a refused start raises the blank scenario, so the driver keeps no default of its own.

## `public event Action<CScenario>? CRepertoireDraftChanged`

Raised with the held Situation's scenario after an edit, through the marshal the repertoire was built with.
`CRepertoireScenarioChanged` fills the scenario anew, while this one only refreshes what the edit changed.

## `public event Action<CSituation>? CRepertoireSituationChanged`

Raised with the Situation a loaded atlas draft carries, ready for the vignette to paint.

## `public event Action? CRepertoireWorkspaceChanged`

Raised after a workspace notice closed the shown Situation, so a driver reloads what belongs to the workspace.

## `public event Action? CRepertoireQueryCleared`

Raised once an arrival drops the query and the language filter, so a driver empties both controls.

## `private readonly CAtelier _cRepertoireAtelier`

The atelier the two vistas start through on every restore.

## `public COccurrence CRepertoireOccurrence { get; }`

The occurrence list, whose panel asks the entry editor's desk whether the entry holds unsaved changes.
It is built over the atelier's entry and portrait ports, so no driver holds either.

## `public CImage CRepertoireImage`

The scenario's picture gates, built fresh over the repertoire's desk as the editor builds its own.

## `public CVideo CRepertoireVideo`

The scenario's video gates, built fresh over the repertoire's desk.

## `public bool CRepertoireScenarioShown`

True while the situation side is in front and in edit mode, so the scenario shows.
The other mode flags follow the same two facts: which side is in front and whether it edits.
`CRepertoireVignetteShown` holds for the situation side outside edit mode.
`CRepertoireDisplayShown` holds for the occurrence side outside edit mode.
`CRepertoireEditorShown` holds while the occurrence list is in edit mode.
`CRepertoireScribeChecked` holds while either editor shows, and `CRepertoireViewerChecked` holds otherwise.
`CRepertoireModeEnabled` holds on the occurrence side, and on the situation side follows its panel.
`CRepertoireBinEnabled` holds only on the situation side, where it follows its panel.
`CRepertoireVignetteHeld` and `CRepertoireVignetteBlank` say whether the situation list has a chosen row.
`CRepertoireStoreEnabled` follows the entry editor while it shows, and the scenario desk otherwise.

## `public bool CRepertoirePressAllowed`

Print takes an entry on display or a chosen Situation on the vignette.
Export, as `CRepertoirePortraitAllowed` says, takes only an entry on display.

## `private LQuillSituation? LRepertoireQuill`

The scenario's typed edits over the desk's live tenure, or null while no tenure runs or the desk fills.

## `private bool LRepertoireOccurrenceSide`

The occurrence side is in front exactly when the occurrence list has a chosen row or is in edit mode.
No driver reads it, since the mode flags it feeds answer every question a driver asks.

## `private bool LRepertoireRowShown`

True while the vignette shows a chosen Situation, the only case a missing row clears.

## `private bool LRepertoireRowHeld`

True while either list has a chosen row, so a fresh start opens an occurrence.

## `internal CSituationDraft? LRepertoireScenarioRead()`

Reads the Situation the desk holds, which persists the tenure first and so can throw.
A throw reports `Situation.HoldFailed` through the envoy and reads as null.
`CRepertoireDraftChanged` hands it on each draft bulletin, so the desk never hands a driver a draft.

## `private void LRepertoireDraftResonate()`

Answers the desk's draft notice with the held Situation read afresh.
A notice with no Situation to show raises nothing, so the driver never redraws from nothing.

## `private void LRepertoireClose()`

The repertoire's part of the window's exit gate `CAtelier.CAtelierClose`, registered with the workspace at build.
It closes the entry editor and cancels its display's playback.
It leaves the scenario desk alone, as the panel's close always did.

## `private void LRepertoireVignetteShow(CSituation? situation)`

Announces the Situation a loaded draft carries and ignores drafts of other subjects.

## `internal void LRepertoireSituationOpen(long id)`

The navigation's arrival on the repertoire tab.
It shows a Situation that comes from outside the list, keeping the scribe open when either editor was open.
When the query or the filter hides it, the list clears it at once.
Both are then dropped and the Situation is shown again, so an arrival never lands on a blank vignette.
A row that vanished stays cleared, and without a query or filter nothing is dropped.

## `private void LRepertoireSituationShow(long id, bool editing)`

Clears the occurrence side and shows the Situation in the mode the caller read.
The atlas panel loads the row, clears a vanished one and restarts the scenario through its edit event.

## `public void CRepertoireSituationSelect(long? id)`

Shows the clicked Situation once the user agrees to leave unsaved changes.
The mode is read before the question, because a save from the dialog must not drop the scribe.
The voyage is recorded only after the user agreed, so a refused leave keeps the Forward history.
A save from the dialog finishes through `CSessionClose`, so the Situation loads once.

## `private void LRepertoireOccurrenceOpen(long id, bool editing)`

Loads the entry on the occurrence side first, and touches nothing else when it fails or has vanished.
Only a held entry cancels the scenario and closes the situation scribe.
The scribe then reopens on the entry when either editor was open.

## `public void CRepertoireOccurrenceSelect(long? id)`

Shows the chosen entry once the user agrees to leave unsaved changes.
The mode is read before the question, as `CRepertoireSituationSelect` does.

## `public void CRepertoireSituationCreate()`

Starts an occurrence for the chosen row when a row is chosen, else a blank Situation scenario.

## `public void CRepertoireScribeToggle(bool editing)`

Toggles the scribe on the side in front.
Closing the occurrence side falls back to the chosen Situation, and closing the scenario cancels the desk.

## `private void LRepertoireSituationRestore(bool editing)`

Shows the chosen Situation again in the given mode, or clears both lists when none is chosen.

## `private void LRepertoireWorkspaceResonate()`

Answers the workspace notice by closing the shown Situation, then raises `CRepertoireWorkspaceChanged`.

## `private void LRepertoireSituationClose()`

Clears both lists, also when the workspace changes under the panel.

## `private void LRepertoireQueryClear()`

Drops the query and the language filter on the situation vista and raises `CRepertoireQueryCleared`.

## `private void LRepertoireStoredShow(long id)`

Shows the stored Situation outside the scribe after the desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `private void LRepertoireOccurrenceCreate()`

Opens a fresh entry in the editor, already linked to the chosen Situation when there is one.
The engine starts and links it in one call, so the first paint shows the link.
The fresh open clears the occurrence panel, which closes the editor before the blank opens.

## `internal bool LRepertoireLeaveConfirm(bool shown)`

True when nothing is unsaved or the user agrees to leave.
The navigation asks it with `shown` true when the tab is left, so a save shows the stored Situation afterwards.

Asks the envoy only over unsaved changes.
A store finishes the session, and a discard leaves without touching it.
The finish shows the stored Situation only when `shown` holds.

## `private void LRepertoireEntryResonate()`

Answers the chosen entry's notice by refreshing the occurrence draft.
It falls back to the chosen Situation once the occurrence side has closed.
It does nothing without a chosen occurrence, so a whole-set bulletin never clears a scenario or a new entry.
The mode is read before the refresh, so a vanished entry under edit reopens the scenario.

## `public IReadOnlyList<CCatalogSituation> CRepertoireRowsRead()`

Reads the situation list's rows, worded by the engine.
A failed read is shown by the list and answers no rows, so nothing is cleared.
When the vignette shows a chosen Situation the rows no longer list, both lists clear before the rows return.
The clear announces fresh rows, which carry the same Situations, so a driver's nested refill is harmless.

## `public Task<CEnsignSheet<IReadOnlyList<CCatalogSituation>>> CRepertoireRowsLoad(`

Runs the flag fill into the driver's `store`, then answers `CRepertoireRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.

## `public CScenarioLine CRepertoireTitleSet(string text)`

Hands the typed title to the scenario's own edit on the tenure, which defers it.
It answers the typed line, whose hint is the untitled text, since typing ends an unknown mark.

## `public CScenarioLine CRepertoireKindSet(string text)`

Hands the typed kind on as the title gate does, and answers the kind's typed line.

## `public CScenarioLine CRepertoireDescriptionSet(string text)`

Hands the typed description on as the title gate does, and answers the description's typed line.

## `public void CRepertoireSituationDelete()`

Deletes the chosen Situation through the atlas panel, which asks the envoy first.
It does nothing on the occurrence side, whose panel has no delete scope.

## `public Task CRepertoirePortraitPrint()`

Prints the entry on display, else the chosen Situation on the vignette, else nothing.
The entry prints with the label, and the Situation list with the Situation realm's legend.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CRepertoirePortraitExport()`

Exports the entry on display, and does nothing otherwise.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.

## `internal void LRepertoireVistaRestore()`

Starts the `repertoire` vista over Situations by name and the `occurrence` vista over entries by headword.
The atlas takes the first, and the occurrence list takes both, since its rows follow the chosen Situation.
The entry editor takes the occurrence vista, so an entry it stores lands in that list.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.
The list owners then carry their held queries into the fresh vistas and attach their observers.
The atlas hands the workspace answer in, and the occurrence list gets the roll and chosen-entry answers.
