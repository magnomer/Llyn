# Taxonomy

The drivers of the taxonomy panel, which browses the workspace by Tag.
It has three columns, a tag catalog, an entry list of the chosen Tag, and a reader or editor.
It stands apart from `Tenor` because the question differs, though the shape is the same.

## `QTaxonomy`

Drives the taxonomy panel and owns when it starts and stops.
The panel itself is the veneer's `PTaxonomy` page, which the window places.
The tag catalog is painted here, and the entry list beside it by its own driver `QMembership`.
The print and export command bindings sit on the page, so the rail's buttons inside it reach them.
The ordering picker's menu hangs under the whole tag search bar.
Each named part is pulled from the page by its contract ID on every read.

`QTaxonomyIntroduce` puts the panel to work through the Conduct taxonomy, which builds its own editor.
Only the medium knows its dispatcher, so the marshal for the area's notices is built here.
The picker and the filter get the tag aperture, titled `Funnel` and `Lattice`.
The ordering menu is built once, here.
The entry list driver is introduced with the area, so it subscribes its own rows and opening.
The tag catalog gets its row fill here, where its source is set.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.
The window fills the panel when the workspace opens, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

`QTaxonomyExitRefine` releases the editor's player, so none outlives the window.
It calls no gate.
The window's exit gate `CAtelierClose` closes the editor and stops its playback in Conduct.

`QTaxonomyPressRefine` keeps print live exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

Print and export both let the engine build the document from the vista and stored rows.
Nothing is read back from the screen.
The export gate asks for the file and the format through the envoy.

`QTaxonomyStoreRefine` enables store only while the editor's draft can be stored.
`QTaxonomyChronicleRefine` lights undo and redo only while the editor has a step to walk.
`QTaxonomyShownCheck` lets a bulletin be acted on only while the page is shown.
`QTaxonomyModeRefine` paints the mode from the shared panel state.
No control's visibility stands in for the mode.

`QTaxonomyWorkspaceRefine` answers the area's workspace event.
The area has already closed the entry, cleared the chosen Tag and raised the rows.
It loads the flags of the workspace that moved.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

`QTaxonomyVistaRefine` answers the workspace opening, after `CTaxonomy` has restored its vistas.
The picker's checked row and the filter mark are drawn from the vista first.
The flags load before any row is built.
The filter's language menu is then built from the languages that load answers.
The catalog is then listed once, so a row keeps the flag it was built with.
Its read raises the entry rows, so the entry list is painted after the flags too.
Its one request is `CTaxonomyRowsLoad`, which runs the flag fill and then answers the rows it paints.

`QDirectoryRefine` lists the Tags the area reads, already in the vista's ordering.
The chosen Tag is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A failed read has already been shown by the area, which then answers no rows.
A read that succeeds makes the area raise the entry rows, so the entry list follows.
The form taking rows paints what a flag-fill load answered, so the paint asks Conduct nothing.

`QDirectoryObserve` hands a clicked row's Tag id to the toggle gate and ignores a click carrying no item.
The gate records the station and toggles, so clicking the chosen tag lets go of it.
That puts the panel back on the whole workspace without a separate control saying so.
The gate raises the rows, so the view rebuilds nothing itself.

`QDirectoryItemRefine` fills one tag row from its item.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The chosen tag stays visible while the eye is on the entries beside it.
Without it the membership list would show a filtered set with nothing on screen saying which filter.

`QDirectoryTagRefine` answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied both queries, so the tag field only shows it.
`QMembership` empties the entry field from the same event.
The field's own handler still hears the change, and its gate finds the query already empty.
The area raises the rows right after, so the catalog is rebuilt without the view asking.

`QTaxonomyFreshObserve` makes a Tag while none is chosen and an Entry under the chosen Tag otherwise.
The one fresh gate decides, and it asks the leave question and any wording through the envoy.
`QTaxonomyScribeObserve` swaps the read view and the editor through the shared panel.

## `QMembership`

The entry list of the taxonomy panel, with the entry search field over it.
It shows the Entries carrying the chosen Tag, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.
It subscribes what it paints itself, so the owner `QTaxonomy` only builds and introduces it.
Each keystroke hands the entry search text to the membership vista, whose announcement refills the entry list.

`QScoutRefine` answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied the entry query, so the field only shows it.
The field's own handler still hears the change, and its gate finds the query already empty.

`QMembershipRefine` answers the entry list's rows event, which every successful Tag read also raises.
So the list follows the catalog, because the chosen Tag may have just changed or vanished.
The first paint arrives the same way, from the catalog's read after the flags are loaded.
Rows sharing a headword arrive numbered, so the reader can tell them apart.
Conduct picks the empty line's key from whether the entry search holds text.
A failed read has already been shown by the area, which then answers no rows.

`QMembershipObserve` hands a clicked row's entry id raw to the row gate, or null when it carries no item.
The gate ignores null, asks the leave question and opens the row.
This panel attaches no station, so the gate records none.

`QMembershipItemRefine` fills one entry row from its item.
It marks the chosen row the same way as the library's catalog.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space.

## `QMembershipItem`

One entry row in `QMembership`.
The id is identity and never displayed.
It is the taxonomy panel's own row, kept apart from the library panel's index row.
The two panels list the same entries but reach them through different questions.
The flag is resolved once for the language and handed to the row, not read from disk by the row.
A row is built while its list is being filled, and reading a file there would stall the fill.
The row announces its chosen flag, so the mark moves without the list being rebuilt.
The epithet defaults to empty, and a null epithet reads as empty.
The shown name is a required init, so no row is built without the engine's numbering.
`QMembershipItemName` is numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`QMembershipItemHeadword` keeps the plain headword for everything that is not display.
`QMembershipItemEpithet` is empty when the setting is off or the entry keeps none.
`QMembershipItemMatch` compares rows with the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.
`QMembershipItemSync` copies the chosen mark onto the kept row.
`QMembershipItemChosen` moves in place, so the list keeps its scroll position.

## `QDirectoryItem`

One tag row in `QDirectory`.
The id is what the membership list is looked up by, and the text is only what is shown.
The chosen flag is read when the row is painted, which tints the tag the membership list stands on.
A pick goes to the gate, and the directory is rebuilt from the rows it answers.
So the item never changes after it is built and announces nothing.

## Inline notes

### QTaxonomy.cs: `private CAtelier _cAtelier = null!;`

The atelier whose catalog the flag load needs.
The kept envoy goes with that load, so a failure is shown.

## Stamp

Commit: `c2f335c331fe`
- `QDirectoryItem.cs` `832f9a698b828323`
- `QMembership.cs` `09cb1443c1a738b5`
- `QMembershipItem.cs` `ce3c25bec96a4c5b`
- `QTaxonomy.cs` `1afc33e14b053fa9`
