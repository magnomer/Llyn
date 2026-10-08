# CXiesheng.cs
Hash: `964470f947356189`

## `public sealed class CXiesheng`

The xiesheng panel's session, the workspace browsed by phonetic series.
It holds the series column's aperture and the entry list, which owns the panel and the entry editor.
It is the yunjing session with one column instead of two, since a series names a cell by itself.
It restores its vistas itself, so no driver holds a port or a vista.

## `private CXiesheng(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Takes the atelier's ports and builds the series column's aperture and the entry list.
The entry list builds the panel and its editor, and its change is raised as this session's change.
It hands the list the series lookup, so the list never reads the column.
It registers its vista restore with the workspace.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CXiesheng CXieshengCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the panel over the atelier, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.
`shownSeam` answers whether the tab is in front, which only the surface knows until `CNavigation` owns it.
`marshal` carries every engine notice onto the driver's thread, so the driver holds no observer.

## `public event Action? CXieshengChanged;`

Raised whenever a column, the page or the mode changed and the driver must copy again.

## `public event Action? CXieshengWorkspaceChanged;`

The workspace changed under the panel, after the series and the chosen entry were let go.
The driver answers it by drawing the flags of the languages again.

## `public CAperture CXieshengGrove { get; }`

The series column's vista with its search, its order, its count and its empty keys.
An empty column prints `Xiesheng.GroveEmpty`, or `Xiesheng.GroveUnmatched` while narrowed.

## `public CEntryList CXieshengKindred { get; }`

The entry list of the chosen series, with its panel, its editor and its press.
An empty list under a chosen series prints `Xiesheng.KindredVacant`, or `Xiesheng.KindredUnmatched` while narrowed.

## `public event Action? CXieshengStemOpened;`

Raised when a series chip's arrival starts, so the driver shows the series search empty.

## `internal bool LXieshengAllowed`

True while a loaded pack declares a series source, the only case the tab is shown in.
The engine answers the verdict.

## `public bool CXieshengStemShown`

True while a chosen series is read as a page rather than an entry.

## `public bool CXieshengDisplayShown`

True while the reader shows an entry for reading.

## `public string CXieshengKindredKey`

The localization key the empty entry list prints, telling no series chosen from a series with nothing.
A chosen series reads the list's aperture key, narrowed or not.

## `private readonly Action<Action> _cXieshengMarshal;`

The medium's marshal the observers run each answer through, since only the medium knows its thread.

## `private bool LXieshengStemChosen`

True while the column points at a series.

## `internal void LXieshengVistaRestore()`

Starts the column's vista off the window posture, ordered by name until the user picks another order.
The aperture carries the search text of the vista before into the fresh one, so a workspace switch keeps it.
The entry list then restores its own vista, and every observer attaches to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LXieshengObserverAttach()`

Attaches the panel's answers to the fresh column, each carried through the marshal.
A Vista, Fanqie or Settings notice on the column raises the column and the rows again.
A Workspace notice on the column goes to `LXieshengWorkspaceResonate`.
The entry list then attaches its own answers to its vista.

## `public static IReadOnlyList<CCatalogOrder> CXieshengOrderRead()`

The orders the column menu offers, in the order it lists them.
It needs no session, so the driver builds the menu once.

## `public IReadOnlyList<CStem> CXieshengGroveRead()`

The series column as the driver copies it, counted into the aperture for the empty verdict.
The engine picks the language and answers nothing while no pack carries series.

## `private IReadOnlyList<LVistaRow> LXieshengKindredFind(LVista kindred)`

The seam the entry list reads its rows through, the entries of the chosen series.
It answers no rows while the column has no vista.

## `public CStemPage CXieshengStemRead()`

The page of the chosen series, or the blank page while the reader shows an entry.
It carries the headword and glyph fonts of the series language, as `CYunjingDiweiRead` does.

## `private void LXieshengWorkspaceResonate()`

Unchooses the series and clears the panel, as a workspace changes.
It then tells the driver through `CXieshengWorkspaceChanged`.

## `public void CXieshengStemSelect(long? id)`

Toggles that series as the chosen one and clears whatever entry was read.
An unsaved draft is first put to the leave question, and nothing changes when the user keeps it.

## `private void LXieshengStemToggle(long stem)`

Toggles the series, clears the entry read, and tells the driver.
It asks nothing, so each caller settles the leave question first.

## `internal void LXieshengStemOpen(string language, string? key)`

The navigation's arrival: opens the series of that language and key, the request a series chip makes.
It first empties the series search and raises `CXieshengStemOpened`, so the driver shows it empty.
The engine finds the series, and a blank key or a series never stored opens nothing.
It toggles the series without the gate's leave question, since the navigation already asked it.
After a discard the draft still reads as changed, so asking again would repeat the dialog.

## `public void CXieshengGlyphSelect(string? character)`

Resolves the glyph picked off a series page to its entry, in the language of the page, and opens it.
The entry opens in the library tab through the navigation.
Nothing opens while no series page shows.
The navigation's jump asks every leave question, so the gate asks none of its own.
The page's language and the resolve are one engine call, so the page is never read whole for its language.
A refused resolve is shown through the catalog's one glyph failure owner, and nothing opens.
