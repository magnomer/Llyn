# CGuild.cs
Hash: `8fc0e9d9ee973930`

## `public sealed class CGuild`

The authors panel, holding the panel state over the roll, the oeuvre beside it, and the autograph desk.
The tab stands on one side at a time.
The source side is in front while a Source is chosen in the oeuvre, else the author side.
The author side shows the vita while reading and the autograph while writing.
The leave question is asked once for the whole tab before any switch, through the session.
The routing between the roll and the oeuvre and its mode flags live in `CDiptych`.
The author union lives in `CGuildUnion`.
The panel's delete question is worded under the Guild scope and counted by the vista.
It asks the user through `CEnvoy` and shows every failure through it too.

## `private CGuild(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the autograph desk, the panel over the roll, the oeuvre and the draft session over the desk.
The session asks the leave question through the envoy, and the navigation's tab and every guild gate ask it.
It keeps the marshal, which runs each engine notice's answer on the medium's own thread.
The panel's rows notice refreshes the oeuvre, since the oeuvre follows the chosen Author.
A cleared panel drops the held draft, and an edited row starts one.
The diptych is built right after the session, with the session's start and cancel as its seams.
It has no create seam, so a create always starts a blank Author.
The union is built over the autograph and the roll panel, and reopens a kept Author through `LGuildAuthorOpen`.
The autograph desk's tenure and draft bulletins are then attached, each run through the marshal.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CGuild CGuildCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the panel over the atelier's ports, as every Conduct panel is built.
`shownSeam` answers whether the tab is in front.
`marshal` runs each desk bulletin's answer on the medium's own thread.

## `public event Action? CGuildChanged;`

A verdict moved without a panel notice, so the buttons and areas are read again.

## `public CPanel CGuildPanel { get; }`

The panel over the roll, which holds the chosen Author and the mode of the author side.

## `public COeuvre CGuildOeuvre { get; }`

The source list beside the roll, which follows the chosen Author.

## `public CDesk CGuildAutograph { get; }`

The desk that holds the Author draft being written.

## `public CSession CGuildSession { get; }`

The draft session over the autograph, which the views save, undo and redo through.
A save refuses a blank name through `LGuildAutographCheck` before the engine is asked.

## `public CDiptych CGuildDiptych { get; }`

The routing between the roll and the oeuvre, with the side flags the drivers paint.
Its child side is the source side, which shows the colophon and allows print, since an Author has no page.
Its parent flags show the autograph while writing and the vita while reading.
Its gates create and delete on the author side.

## `public CGuildUnion CGuildUnion { get; }`

The union of the written Author into a kept one, which the autograph driver reads and drives.

## `public bool CGuildVitaHeld`

Whether the vita has an Author to show, meaning one stored and not being written.

## `public bool CGuildModeEnabled`

The mode toggle is live on the author side while a stored Author is chosen or a draft is open.
The orphan row is chosen with nothing to read or write, so it leaves the toggle dead.

## `public bool CGuildBinEnabled`

The delete button is live on the author side only while a stored Author is chosen.
The orphan row and a fresh draft hold no stored Author, so both leave it dead.

## `public bool CGuildStoreEnabled`

The store button is live on the author side while the autograph desk reports its draft storable.
The desk's tenure answers the verdict, so the guild combines no state fields itself.

## `private bool LGuildRowShown`

A row is chosen and read, not written, so a roll that drops it closes the panel.

## `private long? LGuildAuthorStored`

The id of the chosen Author while it is stored, else null for no choice and for the orphan row.

## `private bool LGuildAuthorHeld`

Whether a stored Author is chosen.

## `private bool LGuildAuthorShown`

Whether the author side has something to show, a stored Author or a draft being written.

## `internal void LGuildVistaRestore()`

Starts the roll and oeuvre vistas through the atelier and binds the panel, the oeuvre and the desk to them.
The panel's aperture carries the roll's held query into the fresh vista, as the oeuvre carries its own.
So the driver never replays it.
It then attaches the observers, so a workspace change rebinds them to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LGuildObserverAttach()`

The guild's observer plan, each answer run through the marshal.
A Vista notice raises the roll, and the oeuvre attaches its own Vista plan.
A Workspace notice closes the Author through the diptych's `LDiptychEntryClose`, which raises no event.
An Author, Reference, Example or Entry notice reads the roll and the counts again.

## `public static IReadOnlyList<CCatalogOrder> CGuildOrderRead()`

The orderings of the roll's order menu, in the order the menu lists them.
The driver builds the menu from it once.

## `public CGuildRoll CGuildRollRead()`

The roll as the view lists it, with its empty verdict, the vita and the kind menu.
Each work count is worded through the engine.
It closes both lists through the diptych when a chosen row being read is no longer listed.
The vita is read after that close, so it never shows an Author the panel just left.
The engine builds the sheet, and `COeuvre.LOeuvreVitaRead` maps it to its shape.

## `private void LGuildCatalogResonate()`

Something cited or credited changed, so the roll and the counts are read again.
The oeuvre's draft is refreshed only while a Source is in front.
It then raises `CGuildChanged`, so the buttons are read again.
It answers an engine notice through the marshal, so it takes the `Resonate` ending.

## `public void CGuildAuthorSelect(long? id)`

A click on the roll or a fellow asks once for the tab, then opens the Author on its side.
Once the leave is settled, the navigation records the station the click leaves.
The mode carries over, so an Author clicked while writing opens in the autograph.

## `internal void LGuildAuthorOpen(long id)`

The navigation's arrival opens the Author without asking, since the navigation has already asked.

## `private void LGuildAuthorOpen(long? id, bool editing)`

Drops the held draft, clears the oeuvre, sets the carried mode, then loads the Author.
The orphan row is never written, so the mode is dropped for it.
The engine's `LVista.LVistaStoredCheck` tells a stored Author from the orphan row.

## `private void LGuildStoredShow(long id)`

The autograph stored this Author, so the roll stands on it and a fresh tenure opens over it.

## `public void CGuildSourceSelect(long? id)`

A click on the oeuvre asks once for the tab, drops the held draft, then opens the colophon.

## `public void CGuildScribeToggle(bool editing)`

The mode toggle of the author side, ignored while a Source is in front.
Turning to writing with no stored Author clears the panel, because there is nothing to write.
Leaving the autograph drops its held draft after the panel state has reloaded the Author.

## `internal void LGuildScribeRestore(bool editing)`

Reopens the side the last session ended on, but only the reading side while no Author is stored.

## `private bool LGuildEditCheck(bool editing)`

Whether the requested mode may open, since writing needs a stored Author and reading needs none.

## `private bool LGuildAutographCheck()`

Whether the held draft may be stored, telling the user `Guild.NameBlank` when the engine refuses it.
`LDeskReadyCheck` of the desk decides, so no draft state is read here.

## `public Task CGuildPortraitPrint()`

Prints the Source read in the colophon, since an Author has no page to print.
The page is worded with the Source realm's legend.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public void CGuildNameSet(string name)`

A keystroke in the name field.
The raw name goes to the autograph's quill, which defers it.
Nothing is written while no tenure is held or while the desk fills its own fields.
