# Guild

This folder holds the authors panel with its session, roll, union and the source list beside it.
It also holds the vita sheet with its co-authors.

## `CGuild`

The authors panel, holding the panel state over the roll, the oeuvre beside it, and the autograph desk.
The tab stands on one side at a time.
The source side is in front while a Source is chosen in the oeuvre, else the author side.
The author side shows the vita while reading and the autograph while writing.
The leave question is asked once for the whole tab before any switch, through the session.
The routing between the roll and the oeuvre lives in `CDiptych`, and the author union in `CGuildUnion`.
The diptych has no create seam, so a create always starts a blank Author.
The child side is the source side, which shows the colophon and allows print, since an Author has no page.
A cleared panel drops the held draft, and an edited row starts one.
The panel's rows notice refreshes the oeuvre, since the oeuvre follows the chosen Author.
It restores its vistas last, so a built area already stands on started vistas.
The panel's delete question is worded under the Guild scope and counted by the vista.

## `CGuild` verdicts

The mode toggle is live on the author side while a stored Author is chosen or a draft is open.
The orphan row is chosen with nothing to read or write, so it leaves the toggle dead.
The delete button is live on the author side only while a stored Author is chosen.
The orphan row and a fresh draft hold no stored Author, so both leave it dead.
The store button follows the autograph desk's tenure, so the guild combines no state fields itself.
A save refuses a blank name through `LGuildAutographCheck` before the engine is asked.
That check tells the user `Guild.NameBlank`, and `LDeskReadyCheck` decides.

## `LGuildRowShown`

A row is chosen and read, not written, so a roll that drops it closes the panel.

## `LGuildVistaRestore`

Starts the roll and oeuvre vistas and binds the panel, the oeuvre and the desk to them.
The aperture carries the roll's held query across, so the driver never replays it.
It then attaches the observers, so a workspace change rebinds them to the fresh vistas.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `LGuildObserverAttach`

A Vista notice raises the roll, and the oeuvre attaches its own Vista plan.
A Workspace notice closes both lists through `LDiptychEntryClose`.
An Author, Reference, Example or Entry notice reads the roll and the counts again.

## `CGuildRollRead`

The roll as the view lists it, with its empty verdict, the vita and the kind menu.
It closes both lists through the diptych when a chosen row being read is no longer listed.
The vita is read after that close, so it never shows an Author the panel just left.
The engine builds the sheet, and `COeuvre.LOeuvreVitaRead` maps it.

## `LGuildCatalogResonate`

Something cited or credited changed, so the roll and the counts are read again.
The oeuvre's draft is refreshed only while a Source is in front.
It then raises `CGuildChanged`, so the buttons are read again.

## `CGuildAuthorSelect`

A click on the roll or a fellow asks once for the tab, then opens the Author on its side.
Once the leave is settled, the navigation records the station the click leaves.
The mode carries over, so an Author clicked while writing opens in the autograph.
`LGuildAuthorOpen` from the navigation opens without asking, since the navigation has already asked.

## `LGuildAuthorOpen`

Drops the held draft, clears the oeuvre, sets the carried mode, then loads the Author.
The orphan row is never written, so the mode is dropped for it.
`LVista.LVistaStoredCheck` tells a stored Author from the orphan row.

## `LGuildStoredShow`

The autograph stored this Author, so the roll stands on it and a fresh tenure opens over it.

## `CGuildSourceSelect`

A click on the oeuvre asks once for the tab, drops the held draft, then opens the colophon.

## `CGuildScribeToggle`

Ignored while a Source is in front.
Turning to writing with no stored Author clears the panel, because there is nothing to write.
Leaving the autograph drops its held draft after the panel state has reloaded the Author.
`LGuildScribeRestore` reopens the side the last session ended on, but only the reading side while no Author is stored.

## `CGuildPortraitPrint`

Opens the print window on the Source read in the colophon, since an Author has no page to print.
It opens nothing while the author side is in front.
The page is worded with the Source realm's legend.

## `CGuildNameSet`

The raw name goes to the autograph's quill, which defers it.
Nothing is written while no tenure is held or while the desk fills its own fields.

## `CGuildRoll`

The guild's roll answer, ready to paint in one go.
The vita comes with the rows, so the view never reads the sheet a second time.
The kind menu comes too, so the workspace opening paints from one read, each repeated tag dropped.
The vita is the sheet of nobody while no Author is stored.

## `CGuildUnion`

The union of the Author being written into a kept Author, beside the guild's autograph.
It asks the union question through `CEnvoy` and shows a failed fold through `CLedger`.
It reads the stored Author off the roll panel's aperture, so it keeps no choice of its own.
The seam reopens a kept Author on the reading side, which only the guild knows how to do.
A started tenure on the autograph clears the union offer, raised as `CGuildUnionCleared`.
The union section shows only while the autograph holds a stored Author, since a fresh one has nothing to fold.

## `CGuildUnionRead`

The Authors the one being written may fold into.
The engine drops blank text, leaves the Author itself out and caps the list.
The rows are mapped through the oeuvre, as the roll's own rows are.

## `CGuildUnionSelect`

A pick asks before folding the written Author into the kept one.
A confirmed fold reopens the kept Author on the reading side through the seam.
A failure is shown as `Guild.MergeFailed` and leaves the written Author open.
The question puts the written name first and the kept name second.
The engine reads both names off the held tenure and the kept id.

## `COeuvre`

The authors panel's source list owns its rows, empty text, tally and colophon.
The query, the empty verdict and the oeuvre vista sit in the panel's aperture.
The rows are the Sources crediting the chosen Author, the uncredited ones under the orphan row, or all.
It keeps a handle on the roll vista too, because the engine narrows the rows by the roll's state.
Its panel carries the Source keys, no delete scope, and never edits, so it stores nothing on leaving.
It also holds the reference, author, vita and usage maps that other lists share.
The panel's loaded Source raises the colophon, which the source side redraws.
The guild attaches the observer plan after the vistas restore, so it stands on the fresh oeuvre vista.

## `COeuvreEmptyKey`

The plain empty text with no Author chosen, else unmatched while narrowed, else vacant.
It reads narrowed when the aperture's query holds text or the roll filters any language.

## `COeuvreRowsRead`

Reads the rows and hands their count to the aperture.
It closes the panel when a chosen Source is no longer listed.
Before the vistas are restored the engine answers no rows.

## `LOeuvreColophonRead`

The colophon of a Source draft, which the engine builds with its tally and this class shapes.
It is static, so the shelf reads its sheet through the same map without a second panel.

## `LOeuvreKindRead`

The kind menu the authors panel's filter lists, called only by `CGuild.CGuildRollRead`.
It hands the engine's kind list to `CImprint.LImprintKindRead`, so both menus drop a repeated tag alike.

## `LOeuvreAuthorRead`

Maps the guild's roll or union rows to their shape, each work count worded by the engine.
The mark's icon key is chosen here, as Conduct chooses every key the driver looks up.

## `COeuvreUsageRead`

Maps one citing place to its shape.
The reading view's incoming rows build through it too, so both lists word a place alike.

## `COeuvreReferenceRead`

The one map for reference rows, shared with the card and the shelf.
An unset author or year reads `Source.Unset`.
A missing author credit keeps the unknown mark, while a present credit is mapped as certain.

## `CFellow` and `CVita`

`CFellow` is an author who shares a source with the author a vita shows.
`CVita` is the sheet the guild shows for its chosen author.
`CVitaNamed` lets the view mute the unnamed wording.
`CVitaFellowShown` and `CVitaUsageShown` decide whether the fellow and citation sections show.

## Stamp

Commit: `c2f335c331fe`
- `CFellow.cs` `471ce5e8f48636dd`
- `CGuild.cs` `87a9802e8f9a358e`
- `CGuildRoll.cs` `4d3eb66139756c29`
- `CGuildUnion.cs` `4a544da4027fce12`
- `COeuvre.cs` `c45470e592d77b77`
- `CVita.cs` `955cf08101155649`
