# CYunjing.cs
Hash: `4792d16fdcd37e66`

## `public sealed class CYunjing`

The yunjing panel's session, the workspace browsed as a rime table by onset and rime.
It holds the initial and rime columns' vistas, the entry list's vista and panel, and the entry editor.
It remembers which column was clicked last, since that column's cell is the one the page reads.
It restores its vistas itself, so no driver holds a port or a vista.

## `private CYunjing(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Takes the atelier's ports and builds the panel's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.
It registers its vista restore and its close with the workspace.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CYunjing CYunjingCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.
`marshal` carries every engine notice onto the driver's thread, so the driver holds no observer.

## `public event Action? CYunjingChanged;`

Raised whenever a column, the page, the tally or the mode changed and the driver must copy again.
Each column, the page and the mode answer it with their own read.

## `public event Action? CYunjingWorkspaceChanged;`

The workspace changed under the panel, after both columns and the chosen entry were let go.
The driver answers it by drawing the flags of the languages again.

## `public event Action? CYunjingDiweiOpened;`

Raised when a fanqie chip's arrival starts, so the driver shows both column searches empty.

## `public CEditor CYunjingEditor { get; }`

The entry editor the reader column opens the chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public CPanel CYunjingPanel { get; }`

The panel state of the entry list, with its mode, its chosen row and its draft.

## `internal bool LYunjingAllowed`

True while a loaded pack carries rime books, the only case the tab is shown in.
The engine answers the verdict.

## `public bool CYunjingDiweiShown`

True while the chosen cell is read as a page rather than an entry.

## `public bool CYunjingDisplayShown`

True while the reader shows an entry for reading.

## `public bool CYunjingEditorShown`

True while the reader shows the editor.

## `public string CYunjingDiweiKey`

The localization key of the page's kind, following the column clicked last.

## `public bool CYunjingShengmuEmpty`

Whether the last initial column read found nothing, so no driver counts rows.

## `public bool CYunjingYunmuEmpty`

Whether the last rime column read found nothing.

## `public bool CYunjingXiaoyunEmpty`

Whether the last entry list read found nothing.

## `public string CYunjingShengmuKey`

The localization key the empty initial column prints, telling a narrowed column from a bare one.

## `public string CYunjingYunmuKey`

The localization key the empty rime column prints, telling a narrowed column from a bare one.

## `public string CYunjingXiaoyunKey`

The localization key the empty entry list prints, telling no cell chosen from a cell with nothing.

## `public CCatalogOrder CYunjingShengmuOrder`

The ordering the initial column is listed in, defaulted by the engine while no vista stands.

## `public CCatalogOrder CYunjingYunmuOrder`

The ordering the rime column is listed in, defaulted by the engine while no vista stands.

## `private string LYunjingVacantKey`

The key a chosen cell with no entry prints, narrowed or not.

## `private bool LYunjingDiweiChosen`

True while the column clicked last points at a cell.

## `private readonly Action<Action> _cYunjingMarshal`

The medium's marshal the observers run each answer through, since only the medium knows its thread.

## `private LVista? LYunjingSide`

The vista of the column clicked last, whose cell the page and the entry list follow.

## `internal void LYunjingVistaRestore()`

Starts the three vistas off the window posture, the two columns and the entry list.
The columns are ordered by name and the list by headword until the user picks another order.
The search text of each vista before it carries into its fresh one, so a workspace switch keeps it.
The list's vista then goes to the panel and the editor, and every observer attaches to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LYunjingObserverAttach(LVista shengmu, LVista yunmu)`

Attaches the panel's answers to the fresh vistas, each carried through the marshal.
A Vista notice on either column raises the columns and the rows again.
A Fanqie, Settings or Reflex notice on the initial column does the same.
A Workspace notice on the initial column goes to `LYunjingWorkspaceResonate`.
The list's own Vista notice raises its rows, and an Entry notice goes to `LYunjingEntryResonate`.
A notice on the chosen entry reads its draft again.

## `public static IReadOnlyList<CCatalogOrder> CYunjingOrderRead()`

The orders both column menus offer, in the order they list them.
It needs no session, so the driver builds the menus once.

## `public IReadOnlyList<CDiwei> CYunjingShengmuRead()`

The initial column as the driver copies it, counted for the empty verdict.

## `public IReadOnlyList<CDiwei> CYunjingYunmuRead()`

The rime column as the driver copies it, counted for the empty verdict.

## `public IReadOnlyList<CVistaRow> CYunjingXiaoyunRead()`

The entry list under the chosen cells, copied through the shared row map and counted for the empty verdict.

## `public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CYunjingXiaoyunLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

Runs the flag fill into the driver's `store`, then answers `CYunjingXiaoyunRead` beside the loaded languages.
The shared rule `CCatalog.LCatalogEnsignLoad` orders the two, so the driver makes one request.

## `public CDiweiPage CYunjingDiweiRead()`

The page of the chosen cell, or the blank page while the reader shows an entry.
The engine picks the tally set, so the map only copies.
The headword and glyph fonts of the cell language come ready, and a blank language answers no font.

## `public void CYunjingShengmuFind(string query)`

Narrows the initial column to that query.

## `public void CYunjingYunmuFind(string query)`

Narrows the rime column to that query.

## `public void CYunjingXiaoyunFind(string query)`

Narrows the entry list to that query.

## `public void CYunjingShengmuSet(CCatalogOrder? order)`

Lists the initial column in that ordering.
The vista keeps the one it has when none is named.

## `public void CYunjingYunmuSet(CCatalogOrder? order)`

Lists the rime column in that ordering.
The vista keeps the one it has when none is named.

## `private void LYunjingWorkspaceResonate()`

Unchooses both columns and clears the panel, as a workspace changes.
It then tells the driver through `CYunjingWorkspaceChanged`.

## `private void LYunjingRowsResonate()`

Answers a column notice by raising the change and refreshing the panel's own rows.

## `private void LYunjingEntryResonate(CBulletin bulletin)`

Answers an entry notice by raising the change and handing the notice to the panel.

## `private void LYunjingClose()`

The panel's part of the atelier's close.
The editor lets its draft go, then the display stops its playback.
`CAtelierClose` runs it through the closure the constructor registers.

## `public void CYunjingDiweiSelect(long? id, bool? final)`

Toggles that cell in its column and makes that column the one the page follows.
Whatever entry was read is cleared.

## `internal void LYunjingDiweiOpen(string language, string kind, string key)`

The navigation's arrival: opens the cell of that language, kind and key, the request a fanqie chip makes.
It first empties both column searches and raises `CYunjingDiweiOpened`, so the driver shows them empty.
The engine finds the cell and its column, and nothing happens for a cell never stored.
Both columns are unchosen first, so the page shows only that cell.

## `public void CYunjingTallyToggle(bool? respelled)`

Shows the tallies in the respelled or the phonemic set, as the switch names.
The engine keeps the choice as a setting.

## `public void CYunjingGlyphSelect(string? character)`

Resolves the glyph picked off a cell page to its entry, in the language of the page, and opens it.
The entry opens in the library tab through the navigation.
Nothing opens while no cell page shows.
The navigation's jump asks every leave question, so the gate asks none of its own.
The page's language and the resolve are one engine call, so the page is never read whole for its language.
A refused resolve is shown through the catalog's one glyph failure owner, and nothing opens.

## `internal string LYunjingFileRead()`

The file name an export of the read entry is offered under.

## `public Task CYunjingPortraitPrint()`

Prints the entry the reader holds, doing nothing while there is none.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CYunjingPortraitExport()`

Exports the entry the reader holds as a portrait file.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.

## `private IReadOnlyList<CDiwei> LYunjingColumnRead(LVista? vista, bool final)`

One column of cells, listed in the language of the cell the page follows.
The engine picks that language and answers nothing while no pack carries rime books.

## `private static CDiweiSection LYunjingSectionRead(LDiweiSection section)`

Copies one page section with its lines and tallies, holding no rule.

## `private static void LYunjingColumnAttach(LVista column, CSubject subject, Action<CBulletin> observer)`

Attaches the observer to one column.
The subject and bulletin maps are the panel's and the atelier's, so the driver names neither engine type.
