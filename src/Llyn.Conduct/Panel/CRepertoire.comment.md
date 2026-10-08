# CRepertoire.cs
Hash: `5af75deed1baf887`

## `public sealed class CRepertoire`

The repertoire panel's session over the situation list, the occurrence list, the playwright and the entry editor.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
The routing between the two lists and its mode flags live in `CDiptych`, shared with the corpus.
Every gate holds only the interaction and reaches the engine through a panel, the playwright or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints or exports.

## `private CRepertoire(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the entry editor, the playwright, the atlas over the playwright's desk and the occurrence list over the editor's desk.
The session defers to the entry editor while the occurrence list edits.
A stored Situation is shown again outside the scribe through `LRepertoireStoredShow`.
The diptych is built right after the session, over the two panels and the playwright desk's start and cancel.
The occurrence panel opens the editor on the entry it edits, and a clear cancels the editor's desk.
The atlas's row notices reach the occurrence panel, whose rows follow the chosen Situation.
It registers the session's editor close with the workspace, so the window's exit gate stops the entry editor.
The leave question is the session's, which the navigation's tab and every repertoire gate ask.
The session's held notice reaches the playwright, which raises the scenario.
It keeps the marshal, so each vista restore attaches the lists' observers on the fresh vistas.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CRepertoire CRepertoireCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the repertoire over the atelier and the panel's own editor.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public event Action? CRepertoireChanged`

Raised when the playwright's desk or the entry editor changes state, so a driver repaints the mode.

## `public event Action<CSituation>? CRepertoireSituationChanged`

Raised with the Situation a loaded atlas draft carries, ready for the vignette to paint.

## `public event Action? CRepertoireWorkspaceChanged`

Raised after a workspace notice closed the shown rows, so a driver reloads what belongs to the workspace.

## `public event Action? CRepertoireQueryCleared`

Raised once an arrival drops the query and the language filter, so a driver empties both controls.

## `private readonly CAtelier _cRepertoireAtelier`

The atelier the two vistas start through on every restore.

## `public CEditor CRepertoireEditor { get; }`

The entry editor of the occurrence side, which the driver wraps for its editor page.

## `public CPlaywright CRepertoirePlaywright { get; }`

The Situation scenario's editor, whose desk the atlas and the session run on.
Drivers reach the scenario's gates and notices through it, as they reach the entry's through `CRepertoireEditor`.

## `public CSession CRepertoireSession { get; }`

The draft session over the playwright's desk, which the views save, undo and redo through.
It defers to the entry editor while the occurrence list edits.

## `public CAtlas CRepertoireAtlas { get; }`

The situation list, whose panel runs on the playwright's desk.

## `public COccurrence CRepertoireOccurrence { get; }`

The occurrence list, whose panel asks the entry editor's desk whether the entry holds unsaved changes.
It is built over the atelier's entry and portrait ports, so no driver holds either.

## `public CDiptych CRepertoireDiptych { get; }`

The routing between the situation list and the occurrence list, with the side flags the drivers paint.
Its gates select, create, toggle the scribe and delete on whichever side is in front.
Its create seam is `LRepertoireOccurrenceCreate`, so a chosen row starts an occurrence.

## `public bool CRepertoireVignetteHeld`

The diptych's flags answer the side questions, and these flags add the rest.
`CRepertoireVignetteHeld` and `CRepertoireVignetteBlank` say whether the situation list has a chosen row.
`CRepertoireStoreEnabled` follows the entry editor while it shows, and the playwright's desk otherwise.
`CRepertoireScenarioEnabled` holds while the playwright's desk runs a tenure, so the scenario area is enabled.

## `public bool CRepertoirePressAllowed`

Print takes an entry on display or a chosen Situation on the vignette.
Export, as `CRepertoirePortraitAllowed` says, takes only an entry on display.

## `private bool LRepertoireRowShown`

True while the vignette shows a chosen Situation, the only case a missing row clears.

## `private void LRepertoireVignetteShow(CSituation? situation)`

Announces the Situation a loaded draft carries, and ignores a draft that carries none.

## `internal void LRepertoireSituationOpen(long id)`

The navigation's arrival on the repertoire tab.
It shows a Situation that comes from outside the list, keeping the scribe open when either editor was open.
When the query or the filter hides it, the list clears it at once.
Both are then dropped and the Situation is shown again, so an arrival never lands on a blank vignette.
A row that vanished stays cleared, and without a query or filter nothing is dropped.

## `private void LRepertoireWorkspaceResonate()`

Answers the workspace notice by closing the shown rows of both lists, then raises `CRepertoireWorkspaceChanged`.

## `private void LRepertoireQueryClear()`

Drops the query and the language filter on the situation vista and raises `CRepertoireQueryCleared`.

## `private void LRepertoireStoredShow(long id)`

Shows the stored Situation outside the scribe after the playwright's desk stores it.
The save button, a tab leave and the window's close finish through it, and a row click does not.

## `private void LRepertoireOccurrenceCreate()`

Opens a fresh entry in the editor, already linked to the chosen Situation when there is one.
The engine starts and links it in one call, so the first paint shows the link.
The fresh open clears the occurrence panel, which closes the editor before the blank opens.
It is the diptych's create seam, so `CDiptychEntryCreate` reaches it while a row is chosen.

## `public IReadOnlyList<CCatalogSituation> CRepertoireRowsRead()`

Reads the situation list's rows, worded by the engine.
A failed read is shown by the list and answers no rows, so nothing is cleared.
When the vignette shows a chosen Situation the rows no longer list, both lists clear before the rows return.
The clear announces fresh rows, which carry the same Situations, so a driver's nested refill is harmless.

## `public Task<CEnsignSheet<IReadOnlyList<CCatalogSituation>>> CRepertoireRowsLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CRepertoireRowsRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows `Situation.LoadFailed` and still answers the rows with no languages.
The driver awaits it from an event handler, where a fault would end the app.

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
