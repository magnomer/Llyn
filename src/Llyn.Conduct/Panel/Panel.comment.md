# Panel

This folder holds shared list slots, panel state and routing, plus favorites, phonology, xiesheng and yunjing areas.
It stands apart from the editor and the engine.
A panel holds only interaction and reaches stored data through ports.
Each area builds its own vista, observers and editor, so no driver wires a subject or holds a port.
Shared patterns repeat across the areas and are stated here once.
A rows-load gate runs the flag fill into the driver's store, then answers the rows read beside the loaded languages.
`CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.
A failed flag fill shows the area's load key and still answers the rows with no languages.
The driver awaits a load from an event handler, so it never faults.
A failed rows read shows the area's load key through the envoy and answers no rows.
A print or export gate goes through `CPortrait`, which opens the session, words the page and shows `Print.Failed` or `Export.Failed`.
The export name is read only once the export starts, and a failed read shows `Export.NameFailed`.
A vista restore carries the held query into the fresh vista, so a workspace switch keeps the search.
An observer attach runs every engine answer through the marshal, since only the medium knows its thread.
Building an area is no user action, so it is no gate on the atelier.

## `CAperture`

The list slot of one browsing panel, which holds the vista the panel stands on.
It owns the rows notice, the query, ordering and filter a driver sets, and the tally chip.
The rows a tab lists are the tab's own, so it only announces that they need re-reading.
The mode and the opening of a row stay with `CPanel`, which reads the vista through this slot.
The slot holds no vista until `CApertureRestore`, so every read before it answers the empty case.
The vista holds only view state, so every stored read goes through the vista port.
A blank load key throws, since it words every failed tally read.
The vacant and unmatched keys word the empty list notice, and a list without them leaves them out.

## `CApertureNarrowed`

Whether the query or the filter hides any row, as the vista answers it.
Only `CCorpus` and `CRepertoire` read it, when an arrival lands on a hidden row.

## `CApertureEmpty`

Whether the last rows read found nothing, which the list shows as its empty notice.
It reads true until an owner hands a count through `CApertureCountSet`.
Only owners that read their rows as a plain list call that.

## `CApertureKey`

The localization key of the empty list notice.
A search with text reads as unmatched and no search as vacant.
It reads empty when the panel gave no keys.

## `CApertureOrderSet`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.
The vista announces only a changed query or filter.

## `CApertureTallyRead`

The tally chip's sentence for the chosen row, how many places cite it.
It is one ShellEngine call, `LVistaPort.LEngineTallyRead`, worded by Core's `LCatalogTallyFormat`.
Before a vista arrives it reads empty.
A fresh draft has no chosen row, so it reads as cited nowhere.
A failure shows the panel's load failure key and answers an empty chip.
Only the situation, example and reference lists call it, since only their vistas count a tally.

## `CApertureRestore`

Takes the vista the window started for the tab.
The held query is carried into the fresh vista first.
The stored split stays as it is, since only a panel's `CPanelVistaRestore` switches it off.

## `CApertureObserverAttach`

Attaches a driver's observer to the vista's notices on a subject.
Each notice is copied into a Conduct notice before the observer sees it.
The wrapped delegate is never detached, because the vista lives as long as the tab.
`CApertureChosenAttach` does the same, but only for notices about the chosen row.

## `CDiptych`

The routing between a parent list and the child list whose rows follow the parent's chosen row.
The corpus, the repertoire, the shelf and the guild share it, so their two-sided mode lives in one place.
The two panels' chosen rows and editing flags are the mode, and the drivers only follow.
The child side is in front exactly when the child list has a chosen row or is in edit mode.
It asks the leave question through the owner's session, which asks the user through the envoy.
Seams stand in for the owner's desk start, desk cancel and child create, which differ per panel.
The create seam may be null, and then a create always starts a blank parent entry.
The owner builds it right after its session.
The mode toggle holds on the child side, and on the parent side follows the parent panel.
The bin holds only on the parent side.

## `CDiptychParentSelect`

Shows the clicked parent row once the user agrees to leave unsaved changes.
The mode is read before the question, because a save from the dialog must not drop the scribe.
The voyage is recorded only after the user agreed, so a refused leave keeps the Forward history.
A save from the dialog closes the desk through the session, so the row loads once.

## `LDiptychParentShow`

Clears the child side and shows the parent row in the mode the caller read.
The parent panel loads the row, clears a vanished one and restarts the owner's desk through its edit event.
The owner's arrival from the navigation shows its row through it.

## `CDiptychChildSelect`

Shows the chosen child row once the user agrees to leave unsaved changes.
The mode is read before the question, as the parent select does.
The child row loads first, and a failed open skips the desk cancel and parent mode change.
Only a held child row cancels the owner's desk and closes the parent scribe.
The scribe then reopens on the child row when either side was editing.

## `CDiptychEntryCreate`

Opens a child entry through the create seam while either list has a chosen row.
Otherwise, or without a create seam, it starts a blank parent entry.
Both ask the leave question first.

## `CDiptychScribeToggle`

Toggles the scribe on the side in front.
Closing the child side falls back to the chosen parent row, or clears both lists without one.
Closing the parent scribe cancels the owner's desk.

## `LDiptychEntryClose`

Clears both lists.
The owner calls it when the workspace changes and when its rows no longer list the shown row.

## `LDiptychChildResonate`

Answers the chosen child row's notice by refreshing the child draft.
The child list's observers call it through the owner's marshal.
It does nothing without a chosen child row, so a whole-set bulletin never clears a fresh entry.
The mode is read before the refresh, so a vanished child row under edit reopens the parent scribe.
It falls back to the chosen parent row once the child side has closed.

## `CDiptychEntryDelete`

Deletes the chosen parent row through the parent panel's bin, which asks the envoy first.
It does nothing on the child side, whose panel has no delete scope.

## `CEntryList`

The entry list beside a chooser column, with its vista, its panel and its entry editor.
The xiesheng and yunjing panels share it, so the list, the reader and the press live in one place.
The owner keeps the chooser columns and hands the list one seam that finds its rows.
The scope names the tab and prefixes the load key.
The list name names the vista and the empty keys.
The allowed check answers whether the tab is shown, which only the owner's pack check knows.
The seam finds the rows under the owner's chosen cell, so the list never reads a column.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.
It registers the draft check and its close with the workspace, and the owner registers the vista restore.
It starts no vista on build, since the owner restores the columns and the list together.
The list read counts its rows for the empty verdict, and the load task runs it after the flag fill.
A throw there would fault the driver, so a failure answers no rows.

## `LEntryListRestore`

Starts the list's vista off the window posture, ordered by headword until the user picks another order.
The vista then goes to the editor.
The owner runs it inside its own restore, after its columns started.

## `LEntryListAttach`

The list's own Vista notice raises its rows, and an Entry notice raises the change and goes to the panel.
A notice on the chosen entry reads its draft again.
The owner runs it after its column observers, so the attach order stays the one before the split.

## `LEntryListResonate`

Answers a column notice by raising the change and refreshing the panel's own rows.
`CEntryListChanged` is raised whenever a column or an entry notice changed the list.

## `CFavorite`

The favorites panel, holding the marked entries and the panel over their vista.
It finds the rows, while the panel's aperture takes the query, order and language filter.
Its panel loads, edits and deletes the chosen entry, and its own editor edits it.
It is worded under the `Scribe` scope and finishes through the editor.
It restores its vista last, so a built panel already stands on a started vista.
It registers its vista restore and its close with the workspace.
The panel asks the editor's desk before it leaves an entry.
A cleared panel empties the editor, and an edited row opens in it.
The editor's display follows the panel, so each draft it loads opens the reading view.
The rows come already filtered, sorted and marked, so the list decides nothing about them.
Vista, favorite, reflex and settings notices refill the rows.
An entry notice selects and repaints, and a chosen entry reloads its draft.
The default order is headword before any order is saved.

## `LFavoriteClose`

The favorites' part of the window's exit gate `CAtelier.CAtelierClose`.
It closes the editor and stops the playback the display started.
The driver releases only its recording player.

## `LFavoriteGraspResonate`

Answers a grasp notice by refreshing the rows, but only while they are ordered by grasp.
A grasp change in any other order moves no row, so the list is left alone.

## `LFavoriteWorkspaceResonate`

Answers the workspace notice by closing the chosen entry, then raising `CFavoriteWorkspaceChanged`.
The driver reloads its language flags on it.

## `CPanel`

The interaction of one browsing panel, shared by every browse tab and both drivers.
Its mode is the vista's editing flag and its chosen row is the vista's choice, so it copies neither.
Every verdict a driver paints is read off the vista at the moment it is asked.
Every question it puts to the user leaves through `CEnvoy`, so no dialog is known here.
Its list slot is the `CAperture` it composes, and its delete is the `CPanelBin` it composes.
Stored reads go through the vista port, and the data rules stay in the engine.
A blank load key throws, since it words every failed load and tally read.
A done delete closes the panel, so the bin's `CPanelBinDeleted` is wired to `CPanelEntryClose`.
The aperture holds no vista until `CPanelVistaRestore`, so every verdict before it reads as no row and no edit.
The vista holds only view state, so every stored read goes through the vista port.

## `CPanel` seams

The change seam says whether the editor holds unstored changes, read from that editor's own desk.
The finish seam stores the editor's work when the user chose to store on leaving.
It answers whether it went through.
A two-list tab hands in its session's finish, so the whole tab stores as one.
The shown seam says whether the tab is in front, which only the driver's surface knows.
It is consulted when a stored entry is announced, so a hidden tab in edit mode does not adopt it.

## `CPanelDraftChanged`

The draft just loaded, passed on unread for the controllers that show it.
It is internal because a draft is engine data, which no driver receives from Conduct's surface.
`CPanelCleared` means the chosen row closed, so the owner drops the draft its editor held.
`CPanelEdited` means the editor must show this stored entry.

## `CPanelBinEnabled`

Live while a row is chosen, so owners read it as the panel's chosen-row fact too.
The mode switch is live with a chosen row or the edit mode already on.
Print takes a chosen row only while it is read, since a draft being written is not stored yet.

## `CPanelVistaRestore`

Takes the vista the window started for the tab and hands it to the aperture.
A vista opens in its stored mode, which no control yet shows, so edit mode is switched off here.
The navigation restores it for the tab in front once a row is chosen.
Only a panel's vista is switched, so the Tag, Register and column vistas keep their stored mode.

## `CPanelLeaveConfirm`

Whether the panel may leave what it edits.
It asks through the envoy only when something is unstored, which can only be so in edit mode.
Storing runs the finish seam, discarding leaves the dropping to the step that asked, and staying answers false.

## `CPanelEntryClose`

Closes what the panel shows.
Nothing is chosen, the rows re-list and the viewer returns.

## `CPanelEntryCreate`

After the leave question it opens the scribe on nothing.
`CPanelFreshOpen` is the same step without the question, for a two-list tab that already asked for the whole tab.

## `LPanelScribeRestore`

Restores the mode the window closed on, but edit mode only over a chosen row.
The navigation calls it for the tab it restores at startup.

## `CPanelScribeToggle`

A switch to the mode already shown does nothing, so a reloaded editor never drops typing.
Leaving the editor asks first, and a refusal re-shows the editor so the toggle reads right again.
Leaving with a chosen row reloads it into the display, leaving with none closes the panel.
Entering the editor hands the chosen row to it, and with no row the panel closes instead.
`CPanelScribeSet` sets the mode and announces it, asking nothing, so a two-list tab can carry the mode across.

## `CPanelRowSelect`

After the leave question the row is opened.
A panel standing on a tab first records the station the click leaves.
`LPanelStationAttach` hands in the navigation's record, and a sub-panel never gets one.
`LPanelChosenRead` answers the station, and zero says there is no place to come back to.

## `CPanelRowOpen`

Selects and loads a row without the leave question, for a step that already asked.
It answers whether the panel now holds the row.
A refused load is told through the envoy and answers false, and the vista has restored its prior choice.
The vista drops a choice that no longer loads, so the chosen verdict decides the close.

## `CPanelDraftResonate`

The chosen entry changed under the panel, so it is reloaded.
A failed reload shows the panel's load key and repaints nothing.
The close check still runs after a failure, so a panel whose row went away closes.

## `CPanelEntrySelect`

A tab in front, in edit mode and with no chosen row adopts the stored entry as its choice.
That is how a fresh entry becomes the shown one, while an entry under edit keeps its row.
It raises nothing, because the adopted entry's own notice reloads and repaints the panel next.
`CPanelEntryResonate` runs after it, re-listing the rows and repainting the mode.

## `CPanelBin`

The delete request of one browsing panel, composed by `CPanel`.
It reads the chosen row off the panel's `CAperture`, so it holds no vista of its own.
Every question it puts to the user leaves through `CEnvoy`, so no dialog is known here.
A null scope says the panel never deletes, as a list that only points at entries.
A panel without a delete scope or without a vista does nothing.
The question names how many places the delete reaches, which the vista counts.
A record nothing references is a plain question, and one something references is asked with its tally.
A refused delete is told through the envoy, and a done one raises `CPanelBinDeleted`.

## `CPhonology`

The phonology panel's session, holding every entry with its primary pronunciation, the panel and the entry editor.
It finds the rows, while the panel's aperture takes the query, order and language filter.
Its panel is worded under the `Scribe` scope and finishes through the editor.
It restores its vista itself, so no driver holds a port or a vista.
The orders offered need no session, so the driver builds the menu once.
A Vista, Reflex or Settings notice raises the rows again.
The rows are already filtered, sorted, twinned and marked, and the engine fills every epithet.
They answer nothing before the vista is restored.
The count goes to the aperture, so no driver counts rows.
A failed read shows `Sound.LoadFailed`.
`LPhonologyClose` joins the atelier's close.
The editor lets its draft go, then the display stops its playback.
`CPhonologyWorkspaceChanged` follows the panel letting its chosen entry go, and the driver redraws the flags.

## `CXiesheng`

The xiesheng panel's session, the workspace browsed by phonetic series.
It holds the series column's aperture and the entry list, which owns the panel and the entry editor.
It is the yunjing session with one column instead of two, since a series names a cell by itself.
It restores its vistas itself, so no driver holds a port or a vista.
It hands the list the series lookup, so the list never reads the column.
It registers its vista restore with the workspace and its series arrival, `LXieshengStemOpen`, with the navigation.
It restores its vistas last, so a built area already stands on started vistas.
An empty column uses `Xiesheng.GroveUnmatched` with a query, and `Xiesheng.GroveEmpty` without one.
An empty list under a chosen series uses `Xiesheng.KindredUnmatched` with a query, and `Xiesheng.KindredVacant` without one.
No series chosen reads a different key from a series with nothing.
The tab is allowed only while a loaded pack declares a series source, and the engine answers the verdict.

## `LXieshengVistaRestore`

Starts the column's vista off the window posture, ordered by name until the user picks another order.
The entry list then restores its own vista, and every observer attaches to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `LXieshengObserverAttach`

A Vista, Fanqie or Settings notice on the column raises the column and the rows again.
A Workspace notice on the column goes to `LXieshengWorkspaceResonate`.
A StemFold notice raises `CXieshengChanged`, so the page redraws a member's fold.
Plain Fold notices are not heard, since entry and card folds change nothing on the series page.

## `CXieshengStemRead`

The page of the chosen series, or the blank page while the reader shows an entry.
It carries the headword and glyph fonts of the series language, as `CYunjingDiweiRead` does.
Its members are mapped by `CStemMember.LStemMemberRead`, from the one engine call that reads the page.

## `CXieshengFoldToggle`

The user's toggle of one member's "More readings" on the shown series page, stored apart from its entry page.
The engine finds the member's entry and raises the StemFold notice the page redraws by.
It answers false with no character, while no series page shows, or when the store refuses.
A refusal shows `Reflex.SpreadFailed`, the failure the entry page's toggle shows.
A bare member answers true, though the engine writes nothing for it.

## `CXieshengStemSelect`

Toggles that series as the chosen one and clears whatever entry was read.
An unsaved draft is first put to the leave question, and nothing changes when the user keeps it.
`LXieshengStemToggle` asks nothing, so each caller settles the leave question first.

## `LXieshengWorkspaceResonate`

Unchooses the series and clears the panel, as a workspace changes.
It then tells the driver through `CXieshengWorkspaceChanged`.

## `CYunjing`

The yunjing panel's session, the workspace browsed as a rime table by onset and rime.
It holds the initial and rime columns' apertures and the entry list, which owns the panel and the entry editor.
It remembers which column was clicked last, since that column's cell is the one the page reads.
It restores its vistas itself, so no driver holds a port or a vista.
It hands the list the cell lookup, so the list never reads a column.
It restores its vistas last, so a built area already stands on started vistas.
An empty column uses its unmatched key with a query, and its empty key without one.
An empty list under a chosen cell uses `Yunjing.XiaoyunUnmatched` with a query, and `Yunjing.XiaoyunVacant` without one.
No cell chosen reads a different key from a cell with nothing.
The tab is allowed only while a loaded pack carries rime books, and the engine answers the verdict.
The page's kind key follows the column clicked last.

## `LYunjingVistaRestore`

Starts both columns' vistas off the window posture, ordered by name until the user picks another order.
The entry list then restores its own vista, and every observer attaches to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `LYunjingObserverAttach`

A Vista notice on either column raises the columns and the rows again.
A Fanqie, Settings or Reflex notice on the initial column does the same.
A Workspace notice on the initial column goes to `LYunjingWorkspaceResonate`.

## `LYunjingSide`

The vista of the column clicked last, whose cell the page and the entry list follow.
The entry list's seam answers no rows while either column has no vista.

## `CYunjingDiweiRead`

The page of the chosen cell, or the blank page while the reader shows an entry.
The engine picks the tally set, so the map only copies.
The headword and glyph fonts of the cell language come ready, and a blank language answers no font.
The column reads list cells in the language of the cell the page follows.

## `CYunjingDiweiSelect`

Toggles that cell in its column and makes that column the one the page follows.
Whatever entry was read is cleared.
An unsaved draft is first put to the leave question, and nothing changes when the user keeps it.
`LYunjingDiweiToggle` asks nothing, so each caller settles the leave question first.

## `LYunjingDiweiOpen`

The navigation's arrival opens the cell of that language, kind and key, the request a fanqie chip makes.
It first empties both column searches and raises `CYunjingDiweiOpened`, so the driver shows them empty.
The engine finds the cell and its column, and a cell never stored opens nothing.
Both columns are unchosen first, so the page shows only that cell.
It toggles the cell without the gate's leave question, since the navigation already asked it.
After a discard the draft still reads as changed, so asking again would repeat the dialog.

## `CYunjingTallyToggle`

Shows the tallies in the respelled or the phonemic set, as the switch names.
The engine keeps the choice as a setting.
A failed save is shown, and the remembered choice stays as saved.

## `CYunjingGlyphSelect`

Resolves the glyph picked off a cell page to its entry in the language of the page.
The entry opens in the library tab.
Nothing opens while no cell page shows.
The navigation's jump asks every leave question, so the gate asks none of its own.
The page's language and the resolve are one engine call, so the page is never read whole for its language.
A refused resolve is shown through the catalog's one glyph failure owner, and nothing opens.

## `LYunjingWorkspaceResonate`

Unchooses both columns and clears the panel, as a workspace changes.
It then tells the driver through `CYunjingWorkspaceChanged`.

## Stamp

Commit: `c2f335c331fe`
- `CAperture.cs` `7645b95c3ccac75e`
- `CDiptych.cs` `a0f45daf855a9029`
- `CEntryList.cs` `4e3ce97f8049b525`
- `CFavorite.cs` `54ea336508512b06`
- `CPanel.cs` `9a7bfccdec6e4dd6`
- `CPanelBin.cs` `0d2b96a54f76e6d4`
- `CPhonology.cs` `631e15a40aae7054`
- `CXiesheng.cs` `082a17a504e62c8b`
- `CYunjing.cs` `4ffaeae361caa525`
