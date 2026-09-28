# CYunjing.cs

## `public sealed class CYunjing`

The yunjing panel's session, the workspace browsed as a rime table by onset and rime.
It holds the initial and rime columns' vistas, the entry list's vista and panel, and the entry editor.
It remembers which column was clicked last, since that column's cell is the one the page reads.
It restores its vistas itself, so no driver holds a port or a vista.

## `private CYunjing(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Takes the atelier's ports and builds the panel's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.

## `public static CYunjing CYunjingCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public event Action? CYunjingChanged`

Raised whenever a column, the page, the tally or the mode changed and the driver must copy again.

## `public event Action<long>? CYunjingGlyphChosen`

Raised with the entry of a character picked off a cell page.
The driver answers it by opening the entry, until `CNavigation` owns the tab switch.

## `public CEditor CYunjingEditor { get; }`

The entry editor the reader column opens the chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public CPanel CYunjingPanel { get; }`

The panel state of the entry list, with its mode, its chosen row and its draft.

## `public bool CYunjingAllowed`

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

## `private LVista? LYunjingSide`

The vista of the column clicked last, whose cell the page and the entry list follow.

## `public void CYunjingVistaRestore()`

Starts the three vistas off the window posture, the two columns and the entry list.
The columns are ordered by name and the list by headword until the user picks another order.
The list's vista then goes to the panel and the editor.

## `public void CYunjingShengmuAttach(CSubject subject, Action<CBulletin> observer)`

Attaches an observer of that subject to the initial column.

## `public void CYunjingYunmuAttach(CSubject subject, Action<CBulletin> observer)`

Attaches an observer of that subject to the rime column.

## `public IReadOnlyList<CDiwei> CYunjingShengmuRead()`

The initial column as the driver copies it, counted for the empty verdict.

## `public IReadOnlyList<CDiwei> CYunjingYunmuRead()`

The rime column as the driver copies it, counted for the empty verdict.

## `public IReadOnlyList<CVistaRow> CYunjingXiaoyunRead()`

The entry list under the chosen cells, copied through the shared row map and counted for the empty verdict.

## `public CDiweiPage CYunjingDiweiRead()`

The page of the chosen cell, or the blank page while the reader shows an entry.
The engine picks the tally set, so the map only copies.

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

## `public void CYunjingDiweiCancel()`

Unchooses both columns and clears the panel, as a workspace changes.

## `public void CYunjingRowsResonate()`

Answers a column notice by raising the change and refreshing the panel's own rows.

## `public void CYunjingEntryResonate(CBulletin bulletin)`

Answers an entry notice by raising the change and handing the notice to the panel.

## `public void CYunjingDiweiSelect(long? id, bool? final)`

Toggles that cell in its column and makes that column the one the page follows.
Whatever entry was read is cleared.

## `public void CYunjingDiweiOpen(string language, string kind, string key)`

Opens the cell of that language, kind and key, the request a fanqie chip makes.
The engine finds the cell and its column, and nothing happens for a cell never stored.
Both columns are unchosen first, so the page shows only that cell.

## `public void CYunjingTallyToggle(bool? respelled)`

Shows the tallies in the respelled or the phonemic set, as the switch names.
The engine keeps the choice as a setting.

## `public void CYunjingGlyphSelect(string? character)`

Resolves the glyph picked off a cell page to its entry, in the language of the page, and raises it.
Nothing is raised while no cell page shows.
An unsaved draft is asked about through the panel's leave question first.
The page's language and the resolve are one engine call, so the page is never read whole for its language.
A refused resolve is shown through the catalog's one glyph failure owner, and nothing is raised.

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

## `private static void LYunjingObserverAttach(LVista? vista, CSubject subject, Action<CBulletin> observer)`

Attaches the observer to one column.
The subject and bulletin maps are the panel's and the atelier's, so the driver names neither engine type.
