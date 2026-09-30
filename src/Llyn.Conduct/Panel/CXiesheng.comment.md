# CXiesheng.cs

## `public sealed class CXiesheng`

The xiesheng panel's session, the workspace browsed by phonetic series.
It holds the series column's vista, the entry list's vista and panel, and the entry editor.
It is the yunjing session with one column instead of two, since a series names a cell by itself.
It restores its vistas itself, so no driver holds a port or a vista.

## `private CXiesheng(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Takes the atelier's ports and builds the panel's own entry editor.
The panel asks the editor's desk before it leaves an entry, and finishes through the editor.
A cleared panel empties the editor, and an edited row opens in it.

## `public static CXiesheng CXieshengCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.

## `public event Action? CXieshengChanged`

Raised whenever a column, the page or the mode changed and the driver must copy again.

## `public event Action? CXieshengStemOpened;`

Raised when a series chip's arrival starts, so the driver shows the series search empty.

## `public CEditor CXieshengEditor { get; }`

The entry editor the reader column opens the chosen entry in.
The driver wraps it for the editor view and the lectern.

## `public CPanel CXieshengPanel { get; }`

The panel state of the entry list, with its mode, its chosen row and its draft.

## `internal bool LXieshengAllowed`

True while a loaded pack declares a series source, the only case the tab is shown in.
The engine answers the verdict.

## `public bool CXieshengStemShown`

True while a chosen series is read as a page rather than an entry.

## `public bool CXieshengDisplayShown`

True while the reader shows an entry for reading.

## `public bool CXieshengEditorShown`

True while the reader shows the editor.

## `public bool CXieshengGroveEmpty`

Whether the last column read found nothing, so no driver counts rows.

## `public bool CXieshengKindredEmpty`

Whether the last entry list read found nothing.

## `public string CXieshengGroveKey`

The localization key the empty series column prints, telling a narrowed column from a bare one.

## `public string CXieshengKindredKey`

The localization key the empty entry list prints, telling no series chosen from a series with nothing.

## `public CCatalogOrder CXieshengOrder`

The ordering the series column is listed in, defaulted by the engine while no vista stands.

## `private string LXieshengVacantKey`

The key a chosen series with no entry prints, narrowed or not.

## `private bool LXieshengStemChosen`

True while the column points at a series.

## `public void CXieshengVistaRestore()`

Starts the two vistas off the window posture, the column and the entry list.
The column is ordered by name and the list by headword until the user picks another order.
The list's vista then goes to the panel and the editor.

## `public void CXieshengObserverAttach(CSubject subject, Action<CBulletin> observer)`

Attaches an observer of that subject to the series column.
The subject and bulletin maps are the panel's and the atelier's, so the driver names neither engine type.

## `public IReadOnlyList<CStem> CXieshengGroveRead()`

The series column as the driver copies it, counted for the empty verdict.
The engine picks the language and answers nothing while no pack carries series.

## `public IReadOnlyList<CVistaRow> CXieshengKindredRead()`

The entry list of the chosen series, copied through the shared row map and counted for the empty verdict.

## `public CStemPage CXieshengStemRead()`

The page of the chosen series, or the blank page while the reader shows an entry.
It carries the headword and glyph fonts of the series language, as `CYunjingDiweiRead` does.

## `public void CXieshengGroveFind(string query)`

Narrows the series column to that query.

## `public void CXieshengKindredFind(string query)`

Narrows the entry list to that query.

## `public void CXieshengGroveSet(CCatalogOrder? order)`

Lists the series column in that ordering.
The vista keeps the one it has when none is named.

## `public void CXieshengStemCancel()`

Unchooses the series and clears the panel, as a workspace changes.

## `public void CXieshengRowsResonate()`

Answers a column notice by raising the change and refreshing the panel's own rows.

## `public void CXieshengEntryResonate(CBulletin bulletin)`

Answers an entry notice by raising the change and handing the notice to the panel.

## `public void CXieshengStemSelect(long? id)`

Toggles that series as the chosen one and clears whatever entry was read.

## `internal void LXieshengStemOpen(string language, string? key)`

The navigation's arrival: opens the series of that language and key, the request a series chip makes.
It first empties the series search and raises `CXieshengStemOpened`, so the driver shows it empty.
The engine finds the series, and nothing happens for a blank key or a series never stored.

## `public void CXieshengGlyphSelect(string? character)`

Resolves the glyph picked off a series page to its entry, in the language of the page, and opens it.
The entry opens in the library tab through the navigation.
Nothing opens while no series page shows.
The navigation's jump asks every leave question, so the gate asks none of its own.
The page's language and the resolve are one engine call, so the page is never read whole for its language.
A refused resolve is shown through the catalog's one glyph failure owner, and nothing opens.

## `internal string LXieshengFileRead()`

The file name an export of the read entry is offered under.

## `public Task CXieshengPortraitPrint()`

Prints the entry the reader holds, doing nothing while there is none.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public Task CXieshengPortraitExport()`

Exports the entry the reader holds as a portrait file.
The reader is asked for the file and format through the panel's envoy, and a decline exports nothing.
`CPortrait` words the page through the engine and shows `Export.Failed` through the panel's envoy.
