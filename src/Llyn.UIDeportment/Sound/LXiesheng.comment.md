# LXiesheng.cs

## `public sealed class LXiesheng`

The deportment of the xiesheng panel, the workspace browsed by phonetic series.
It holds the series column's vista, the entry list's panel state, and the series and glyph requests.
It is the yunjing deportment with one column instead of two, since a series names a cell by itself.

## `internal LXiesheng(...)`

Takes the engine ports, the editor it shares with the panel, and the shell's three seams.
The panel clears and opens the editor through the notices it raises.
Its loads and clears also go straight to the lectern its view hands in, so the veneer relays no draft.

## `private void LXieshengEditorClear()`

Closes whatever the editor held, as the panel is cleared.

## `private void LXieshengEditorOpen(long id)`

Opens that entry in the editor, as the panel asks for an edit.

## `public event Action? LXieshengChanged`

Raised whenever a column, the page or the mode changed and the driver must copy again.

## `public event Action<string, string>? LXieshengGlyphChosen`

Raised with a character and its language, as one is picked off a series page.

## `public LEditor LXieshengEditor`

The editor the reader column shares with the panel.

## `public LPanel LXieshengPanel`

The panel state of the entry list: its mode, its chosen row and its draft.

## `public bool LXieshengAllowed`

True while a loaded pack declares a series source, the only case the tab is shown in.

## `public bool LXieshengStemShown`

True while a chosen series is read as a page rather than an entry.

## `public bool LXieshengDisplayShown`

True while the reader shows an entry for reading.

## `public bool LXieshengEditorShown`

True while the reader shows the editor.

## `public bool LXieshengGroveEmpty`

True while the series column listed nothing.

## `public bool LXieshengKindredEmpty`

True while the entry list listed nothing.

## `public string LXieshengGroveKey`

The localization key the empty series column prints, telling a narrowed column from a bare one.

## `public string LXieshengKindredKey`

The localization key the empty entry list prints, telling no series chosen from a series with nothing.

## `public CCatalogOrder LXieshengRung`

The ordering the series column is listed in.

## `private string LXieshengVacantKey`

The key a chosen series with no entry prints, narrowed or not.

## `private bool LXieshengLodestarQueried`

True while the series column carries a query.

## `private bool LXieshengSextantQueried`

True while the entry list carries a query.

## `private bool LXieshengStemChosen`

True while the column points at a series.

## `private string LXieshengLanguage`

The language of the chosen series, or the first pack declaring a series source.

## `internal void LXieshengVistaRestore(CAtelier atelier)`

Starts the two vistas off the window posture, the column and the entry list.
The atelier calls it when the factory builds the panel and again after a workspace change.

## `internal void LXieshengVistaRestore(LVista grove, LVista kindred)`

Keeps the two vistas and hands the entry list's to the panel and the editor.

## `public IReadOnlyList<CStem> LXieshengGroveRead()`

The series column as the driver copies it, counted so the empty text knows when to show.

## `internal static IReadOnlyList<CStem> LXieshengGroveBuild(IReadOnlyList<LStem> rows)`

The Conduct copy of the engine's series rows, carrying each chosen mark.

## `private IReadOnlyList<LStem> LXieshengStemFind()`

The series of the language under the column's query and order, or nothing while no pack carries them.

## `public IReadOnlyList<CVistaRow> LXieshengKindredRead()`

The entry list of the chosen series, copied through the shared row map and counted for the empty text.

## `private IReadOnlyList<LVistaRow> LXieshengKindredFind()`

The entry rows the chosen series reaches, narrowed by the list's own query.

## `public CStemPage LXieshengStemRead()`

The page of the chosen series, or the blank page while the reader shows an entry.

## `internal static CStemPage LXieshengPageBuild(LStemPage page)`

The Conduct copy of the engine's series page.

## `public void LXieshengLodestarSet(string query)`

Narrows the series column to that query.

## `public void LXieshengSextantSet(string query)`

Narrows the entry list to that query.

## `public void LXieshengRungSet(CCatalogOrder? order)`

Lists the series column in that ordering, keeping the one it has when none is named.

## `public void LXieshengReset()`

Unchooses the series and clears the panel, as a workspace changes.

## `public void LXieshengRowsUpdate()`

Copies the columns again and lets the panel refresh its own rows.

## `public void LXieshengEntryHandle(CBulletin bulletin)`

Copies the columns again and hands the entry notice to the panel.

## `public void LXieshengStemSelect(long? id)`

Toggles that series as the chosen one and clears whatever entry was read.

## `public void LXieshengStemShow(string language, string key)`

Opens the series of that language and key, the request a series chip makes.
Nothing happens while the series was never stored.

## `public void LXieshengGlyphSelect(string? character)`

Asks the shell for the entry of that character, as one is picked off a series page.
It asks nothing while an unsaved draft is held.

## `public void LXieshengGroveAttach(CSubject subject, Action<CBulletin> observer)`

Attaches an observer of that subject to the series column.
The engine's subject and bulletin are mapped here, so the driver names neither.

## `public Task LXieshengPortraitPrint(CPortraitLabel label, CPressTicket ticket)`

Prints the entry the reader holds, doing nothing while there is none.
The label and ticket are mapped down through `QPortrait`.

## `public Task LXieshengPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)`

Exports the entry the reader holds as a portrait file.
The medium and label are mapped down through `QPortrait`.

## `public string LXieshengFileRead()`

The file name an export of the read entry is offered under.
