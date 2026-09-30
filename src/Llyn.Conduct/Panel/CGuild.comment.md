# CGuild.cs

## `public sealed class CGuild`

The authors panel: the panel state over the roll, the oeuvre beside it, and the autograph desk.
The tab stands on one side at a time.
The source side is in front while a Source is chosen in the oeuvre, else the author side.
The author side shows the vita while reading and the autograph while writing.
The leave question is asked once for the whole tab before any switch.
The panel's delete question is worded under the Guild scope and counted by the vista.
It asks the user through `CEnvoy` and shows every failure through it too.

## `private CGuild(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the autograph desk, the panel over the roll, the oeuvre and the draft session over the desk.
It keeps the marshal, which runs each engine notice's answer on the medium's own thread.
The panel's rows notice refreshes the oeuvre, since the oeuvre follows the chosen Author.
A cleared panel drops the held draft, and an edited row starts one.
A started tenure clears the union offer, raised as `CGuildUnionCleared`.
The autograph desk's tenure and draft bulletins are then attached, each run through the marshal.
It restores its vistas last, so a built area already stands on started vistas.

## `public static CGuild CGuildCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)`

Builds the panel over the atelier's ports, as every Conduct panel is built.
`shownSeam` answers whether the tab is in front.
`marshal` runs each desk bulletin's answer on the medium's own thread.

## `public event Action? CGuildUnionCleared;`

A tenure started on the autograph, so the union search and its offer are emptied.

## `public event Action? CGuildChanged;`

A verdict moved without a panel notice, so the buttons and areas are read again.

## `public CSession CGuildSession { get; }`

The draft session over the autograph, which the views save, undo and redo through.
A save refuses a blank name through `LGuildAutographCheck` before the engine is asked.

## `public bool CGuildVitaHeld`

Whether the vita has an Author to show: one stored and not being written.

## `public bool CGuildModeEnabled`

The mode toggle is live on the author side while a stored Author is chosen or a draft is open.
The orphan row is chosen with nothing to read or write, so it leaves the toggle dead.

## `public bool CGuildStoreEnabled`

The store button is live while the autograph holds a changed draft the engine would store.
The tenure's `LTenureReadyCheck` answers whether it would.

## `public bool CGuildUnionShown`

The union section shows only while the autograph holds a stored Author, since a fresh one has nothing to fold.

## `private bool LGuildSourceSide`

The source side is in front exactly when the oeuvre has a chosen row.

## `private bool LGuildRowShown`

A stored row is chosen and read, not written, so a roll that drops it closes the panel.

## `internal void LGuildVistaRestore()`

Starts the roll and oeuvre vistas through the atelier and binds the panel, the oeuvre and the desk to them.
The roll's held query carries into the fresh vista, as the oeuvre carries its own.
So the driver never replays it.
It then attaches the observers, so a workspace change rebinds them to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `private void LGuildObserverAttach()`

The guild's observer plan, each answer run through the marshal.
A Vista notice raises the roll, and the oeuvre attaches its own Vista plan.
A Workspace notice closes the Author.
An Author, Reference, Example or Entry notice reads the roll and the counts again.

## `public static IReadOnlyList<CCatalogOrder> CGuildOrderRead()`

The orderings of the roll's order menu, in the order the menu lists them.
Both media build the menu from it once.

## `public CGuildRoll CGuildRollRead()`

The roll as the view lists it, with its empty verdict and the vita.
Each work count is worded through the engine.
It closes the panel when a chosen row being read is no longer listed.
The vita is read after that close, so it never shows an Author the panel just left.
The engine builds the sheet, and `COeuvre.LOeuvreVitaRead` maps it to its shape.
The same sheet feeds the count chips of the autograph, which show the Author being written.

## `public void CGuildOrderSet(CCatalogOrder? order)`

The order menu of the roll, where no order keeps the one the vista holds.

## `internal bool LGuildLeaveConfirm()`

Asks the leave question when the tab holds unsaved work, and answers whether the tab may be left.
A store answer finishes the draft first, and a refused store keeps the tab.

## `private void LGuildAuthorClose()`

Closes the oeuvre and the roll panel, as a workspace change or a vanished row asks.
The observer plan attaches it straight to the Workspace notice, since the close refreshes the rows of both panels.
No event is raised, since the panels' own row notices repaint every list.

## `private void LGuildCatalogResonate()`

Something cited or credited changed, so the roll and the counts are read again.
The colophon is reloaded only while a Source is in front, so no split preference is touched otherwise.
It answers an engine notice through the marshal, so it takes the `Resonate` ending.

## `public void CGuildAuthorSelect(long? id)`

A click on the roll or a fellow: asks once for the tab, then opens the Author on its side.
Once the leave is settled, the navigation records the station the click leaves.
The mode carries over, so an Author clicked while writing opens in the autograph.

## `internal void LGuildAuthorOpen(long id)`

The navigation's arrival: opens the Author without asking, since the navigation has already asked.

## `private void LGuildAuthorOpen(long? id, bool editing)`

Drops the held draft, clears the oeuvre, sets the carried mode, then loads the Author.
The orphan row is never written, so the mode is dropped for it.
The engine's `LVista.LVistaStoredCheck` tells a stored Author from the orphan row.

## `private void LGuildStoredShow(long id)`

The autograph stored this Author, so the roll stands on it and a fresh tenure opens over it.

## `public void CGuildSourceSelect(long? id)`

A click on the oeuvre: asks once for the tab, drops the held draft, then opens the colophon.

## `public void CGuildAuthorCreate()`

Starts an Author draft in the autograph with nothing chosen, after asking once for the tab.

## `public void CGuildScribeToggle(bool editing)`

The mode toggle of the author side, ignored while a Source is in front.
Turning to writing with no stored Author clears the panel, because there is nothing to write.
Leaving the autograph drops its held draft after the panel state has reloaded the Author.

## `internal void LGuildScribeRestore(bool editing)`

Reopens the side the last session ended on, but only the reading side while no Author is stored.

## `private bool LGuildAutographCheck()`

Whether the held draft may be stored, telling the user `Guild.NameBlank` when the engine refuses it.
The tenure answers the refusal itself through `LTenureReadyCheck`, so no draft state is read here.

## `public void CGuildAuthorDelete()`

Deletes the chosen Author through the panel's question, and nothing while a Source is in front.

## `public Task CGuildPortraitPrint()`

Prints the Source read in the colophon, since an Author has no page to print.
The page is worded with the Source realm's legend.
The reader is asked for the printer through the panel's envoy, and a decline prints nothing.
`CPortrait` words the page through the engine and shows `Print.Failed` through the panel's envoy.

## `public void CGuildNameSet(string name)`

A keystroke in the name field: the raw name goes to the autograph's quill, which defers it.
Nothing is written while no tenure is held or while the desk fills its own fields.

## `public IReadOnlyList<CCatalogAuthor> CGuildUnionRead(string typed)`

The Authors the one being written may fold into, as the union list shows them.
The engine drops blank text, leaves the Author itself out and caps the list.

## `public void CGuildUnionSelect(long? id)`

A pick in the union list: asks before folding the written Author into the kept one.
A confirmed fold reopens the kept Author on the reading side, and a failure is shown as `Guild.MergeFailed`.

## `private bool LGuildUnionConfirm(long kept)`

Asks the union question with the written name first and the kept name second.
The engine reads both names off the held tenure and the kept id, which Conduct hands on unread.
