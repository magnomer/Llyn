# LGuild.cs

## `public sealed class LGuild`

The deportment of the authors panel: the panel state over the roll, the oeuvre beside it, and the autograph desk.
The tab stands on one side at a time.
The source side is in front while a Source is chosen in the oeuvre, else the author side.
The author side shows the vita while reading and the autograph while writing.
The leave seam is the window's discard dialog, asked once for the whole tab before any switch.
The removal seam is the window's delete question, worded by how many Sources credit the Author.
The union seam is the window's merge question, worded by the two names.

## `public event Action? LGuildChanged;`

A verdict moved without a panel notice, so the buttons and areas are read again.

## `public event Action<string>? LGuildRefused;`

A request the deportment itself refused, named by the localization key the window shows.

## `public bool LGuildSourceSide`

The source side is in front exactly when the oeuvre has a chosen row.

## `public bool LGuildVitaHeld`

Whether the vita has an Author to show: one stored and not being written.

## `public bool LGuildModeEnabled`

The mode toggle is live on the author side while a stored Author is chosen or a draft is open.
The orphan row is chosen with nothing to read or write, so it leaves the toggle dead.

## `public bool LGuildStoreEnabled`

The store button is live while the autograph holds a named draft that differs from what is stored.

## `public bool LGuildUnionShown`

The union section is shown while the autograph holds a stored Author, since a fresh one folds into nothing.

## `private long LGuildAuthorId`

The id of the stored Author the roll stands on, zero on the orphan row and on nothing.

## `private IReadOnlyList<LCatalogAuthor> LGuildRollApply(IReadOnlyList<LCatalogAuthor> rows)`

Counts the rows and drops a chosen row the roll no longer lists, unless it is being written.
The rows cross as a parameter, so no local carries an engine answer into the clear.

## `public LVita LGuildVitaRead()`

The read sheet of the chosen Author, or the sheet of nobody while none is stored.
The same sheet feeds the count chips of the autograph, which show the Author being written.

## `public IReadOnlyList<LCatalogAuthor> LGuildUnionRead(string typed)`

The Authors the typed name matches, the one being written left out, at most a handful.

## `private bool LGuildDeleteConfirm()`

Asks the removal seam with the Source count of the chosen Author, read from the engine at that moment.

## `public void LGuildCatalogUpdate()`

Something cited or credited changed, so the roll and the counts are read again.
The colophon is reloaded only while a Source is in front, so no split preference is touched otherwise.

## `public void LGuildRowSelect(long? id)`

A click on the roll or a fellow: asks once for the tab, then opens the Author on its side.
The mode carries over, so an Author clicked while writing opens in the autograph.

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

## `public bool LGuildSave()`

Stores the held draft, refusing a blank name under its own key before the engine is asked.

## `public bool LGuildDraftFinish(bool store)`

Finishes the held draft before the panel is left, saving it or dropping it as asked.

## `public void LGuildUnionSelect(long? id)`

Folds the Author being written into the chosen one, behind the union seam, then reads the kept one.

## `public Task LGuildPortraitPrint(LPortraitLegend legend, LPressTicket ticket)`

Prints the Source read in the colophon, since an Author has no page to print.
