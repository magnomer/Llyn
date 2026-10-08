# CYunjing.cs
Hash: `4ffaeae361caa525`

## `public sealed class CYunjing`

The yunjing panel's session, the workspace browsed as a rime table by onset and rime.
It holds the initial and rime columns' apertures and the entry list, which owns the panel and the entry editor.
It remembers which column was clicked last, since that column's cell is the one the page reads.
It restores its vistas itself, so no driver holds a port or a vista.

## `private CYunjing(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Takes the atelier's ports and builds both columns' apertures and the entry list.
The entry list builds the panel and its editor, and its change is raised as this session's change.
It hands the list the cell lookup, so the list never reads a column.
It registers its vista restore with the workspace.
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

## `public CAperture CYunjingShengmu { get; }`

The initial column's vista with its search, its order, its count and its empty keys.
An empty column prints `Yunjing.ShengmuEmpty`, or `Yunjing.ShengmuUnmatched` while narrowed.

## `public CAperture CYunjingYunmu { get; }`

The rime column's vista with its search, its order, its count and its empty keys.
An empty column prints `Yunjing.YunmuEmpty`, or `Yunjing.YunmuUnmatched` while narrowed.

## `public CEntryList CYunjingXiaoyun { get; }`

The entry list under the chosen cell, with its panel, its editor and its press.
An empty list under a chosen cell prints `Yunjing.XiaoyunVacant`, or `Yunjing.XiaoyunUnmatched` while narrowed.

## `public event Action? CYunjingDiweiOpened;`

Raised when a fanqie chip's arrival starts, so the driver shows both column searches empty.

## `internal bool LYunjingAllowed`

True while a loaded pack carries rime books, the only case the tab is shown in.
The engine answers the verdict.

## `public bool CYunjingDiweiShown`

True while the chosen cell is read as a page rather than an entry.

## `public bool CYunjingDisplayShown`

True while the reader shows an entry for reading.

## `public string CYunjingDiweiKey`

The localization key of the page's kind, following the column clicked last.

## `public string CYunjingXiaoyunKey`

The localization key the empty entry list prints, telling no cell chosen from a cell with nothing.
A chosen cell reads the list's aperture key, narrowed or not.

## `private bool LYunjingDiweiChosen`

True while the column clicked last points at a cell.

## `private readonly Action<Action> _cYunjingMarshal`

The medium's marshal the observers run each answer through, since only the medium knows its thread.

## `private LVista? LYunjingSide`

The vista of the column clicked last, whose cell the page and the entry list follow.

## `internal void LYunjingVistaRestore()`

Starts both columns' vistas off the window posture, ordered by name until the user picks another order.
Each aperture carries the search text of the vista before into the fresh one, so a workspace switch keeps it.
The entry list then restores its own vista, and every observer attaches to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LYunjingObserverAttach()`

Attaches the panel's answers to the fresh columns, each carried through the marshal.
A Vista notice on either column raises the columns and the rows again.
A Fanqie, Settings or Reflex notice on the initial column does the same.
A Workspace notice on the initial column goes to `LYunjingWorkspaceResonate`.
The entry list then attaches its own answers to its vista.

## `public static IReadOnlyList<CCatalogOrder> CYunjingOrderRead()`

The orders both column menus offer, in the order they list them.
It needs no session, so the driver builds the menus once.

## `public IReadOnlyList<CDiwei> CYunjingShengmuRead()`

The initial column as the driver copies it, counted into its aperture for the empty verdict.

## `public IReadOnlyList<CDiwei> CYunjingYunmuRead()`

The rime column as the driver copies it, counted into its aperture for the empty verdict.

## `private IReadOnlyList<LVistaRow> LYunjingXiaoyunFind(LVista xiaoyun)`

The seam the entry list reads its rows through, the entries under the cell the page follows.
It answers no rows while either column has no vista.

## `public CDiweiPage CYunjingDiweiRead()`

The page of the chosen cell, or the blank page while the reader shows an entry.
The engine picks the tally set, so the map only copies.
The headword and glyph fonts of the cell language come ready, and a blank language answers no font.

## `private void LYunjingWorkspaceResonate()`

Unchooses both columns and clears the panel, as a workspace changes.
It then tells the driver through `CYunjingWorkspaceChanged`.

## `public void CYunjingDiweiSelect(long? id, bool? final)`

Toggles that cell in its column and makes that column the one the page follows.
Whatever entry was read is cleared.
An unsaved draft is first put to the leave question, and nothing changes when the user keeps it.

## `private void LYunjingDiweiToggle(long cell, bool rime)`

Makes the cell's column the followed one, toggles the cell, clears the entry read, and tells the driver.
It asks nothing, so each caller settles the leave question first.

## `internal void LYunjingDiweiOpen(string language, string kind, string key)`

The navigation's arrival: opens the cell of that language, kind and key, the request a fanqie chip makes.
It first empties both column searches and raises `CYunjingDiweiOpened`, so the driver shows them empty.
The engine finds the cell and its column, and a cell never stored opens nothing.
Both columns are unchosen first, so the page shows only that cell.
It toggles the cell without the gate's leave question, since the navigation already asked it.
After a discard the draft still reads as changed, so asking again would repeat the dialog.

## `public void CYunjingTallyToggle(bool? respelled)`

Shows the tallies in the respelled or the phonemic set, as the switch names.
The engine keeps the choice as a setting.
A failed save is shown, and the remembered choice stays as saved.

## `public void CYunjingGlyphSelect(string? character)`

Resolves the glyph picked off a cell page to its entry, in the language of the page, and opens it.
The entry opens in the library tab through the navigation.
Nothing opens while no cell page shows.
The navigation's jump asks every leave question, so the gate asks none of its own.
The page's language and the resolve are one engine call, so the page is never read whole for its language.
A refused resolve is shown through the catalog's one glyph failure owner, and nothing opens.

## `private IReadOnlyList<CDiwei> LYunjingColumnRead(LVista? vista, bool final)`

One column of cells, listed in the language of the cell the page follows.
The engine picks that language and answers nothing while no pack carries rime books.

## `private static CDiweiSection LYunjingSectionRead(LDiweiSection section)`

Copies one page section with its lines and tallies, holding no rule.
