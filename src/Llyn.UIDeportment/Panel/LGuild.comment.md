# LGuild.cs

## `public sealed class LGuild`

The deportment of the authors panel: the panel state over the roll, the oeuvre beside it, and the autograph desk.
The tab stands on one side at a time.
The source side is in front while a Source is chosen in the oeuvre, else the author side.
The author side shows the vita while reading and the autograph while writing.
The leave seam is the window's discard dialog, asked once for the whole tab before any switch.
The panel's delete question is worded under the Guild scope and counted by the vista.
The union seam is the window's merge question, worded by the two names.
It is sealed, so its public members name only .NET types, Conduct shapes and deportment handles.
Its constructor and vista restore take engine types, so they stay internal.

## `public event Action? LGuildChanged;`

A verdict moved without a panel notice, so the buttons and areas are read again.

## `public event Action<string>? LGuildRefused;`

A request the deportment itself refused, named by the localization key the window shows.

## `public CSession LGuildSession { get; }`

The draft session over the autograph, which the views save, undo and redo through.
A save refuses a blank name through `LGuildAutographCheck` before the engine is asked.

## `public QUnion LGuildUnion { get; }`

The author union, which reads the Author the roll stands on and reopens the kept one.

## `private bool LGuildAutographCheck()`

Whether the held draft is named, raising `LGuildRefused` with `Guild.NameBlank` when not.

## `private bool LGuildSourceSide`

The source side is in front exactly when the oeuvre has a chosen row.

## `public bool LGuildVitaHeld`

Whether the vita has an Author to show: one stored and not being written.

## `public bool LGuildModeEnabled`

The mode toggle is live on the author side while a stored Author is chosen or a draft is open.
The orphan row is chosen with nothing to read or write, so it leaves the toggle dead.

## `public bool LGuildStoreEnabled`

The store button is live while the autograph holds a named draft that differs from what is stored.

## `private long LGuildAuthorId`

The id of the stored Author the roll stands on, zero on the orphan row and on nothing.

## `private IReadOnlyList<LCatalogAuthor> LGuildRollApply(IReadOnlyList<LCatalogAuthor> rows)`

Counts the rows and drops a chosen row the roll no longer lists, unless it is being written.
The rows cross as a parameter, so no local carries an engine answer into the clear.

## `public IReadOnlyList<CCatalogAuthor> LGuildRollRead()`

The roll as the view lists it, each source count worded through the engine.

## `public CVita LGuildVitaRead()`

The read sheet of the chosen Author, or the sheet of nobody while none is stored.
The engine builds the sheet, and `COeuvre.COeuvreVitaRead` maps it to its shape.
The same sheet feeds the count chips of the autograph, which show the Author being written.

## `public void LGuildCatalogUpdate()`

Something cited or credited changed, so the roll and the counts are read again.
The colophon is reloaded only while a Source is in front, so no split preference is touched otherwise.

## `public void LGuildRowSelect(long? id)`

A click on the roll or a fellow: asks once for the tab, then opens the Author on its side.
The mode carries over, so an Author clicked while writing opens in the autograph.

## `public void LGuildRowShow(long id)`

A jump from another panel: opens the Author without asking, since the window has already asked.

## `private void LGuildAuthorOpen(long? id, bool editing)`

Drops the held draft, clears the oeuvre, sets the carried mode, then loads the Author.
The orphan row is never written, so the mode is dropped for it.

## `private void LGuildStoredShow(long id)`

The autograph stored this Author, so the roll stands on it and a fresh tenure opens over it.

## `public void LGuildSourceSelect(long? id)`

A click on the oeuvre: asks once for the tab, drops the held draft, then opens the colophon.

## `public void LGuildFreshStart()`

Starts an Author draft in the autograph with nothing chosen.

## `public void LGuildScribeSet(bool editing)`

The mode toggle of the author side, ignored while a Source is in front.
Turning to writing with no stored Author clears the panel, because there is nothing to write.
Leaving the autograph drops its held draft after the panel state has reloaded the Author.

## `public void LGuildScribeRestore(bool editing)`

Reopens the side the last session ended on, but only the reading side while no Author is stored.

## `public Task LGuildPortraitPrint(CPortraitLegend legend, CPressTicket ticket)`

Prints the Source read in the colophon, since an Author has no page to print.
The legend and ticket are mapped to the engine at the port.
