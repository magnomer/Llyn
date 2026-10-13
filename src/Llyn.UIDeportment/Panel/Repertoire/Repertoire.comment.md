# Repertoire

The Repertoire panel, the view of the shared stock of usage contexts, with its Situation catalog.
It stands apart because a Situation is independent data owned by nothing.
So the panel answers two questions: what this Situation is, and where it is used.
Sub-drivers subscribe what they paint themselves, so the owner only builds and introduces them.

## `QRepertoire`

The panel's driver, holding the surface, the atelier and envoy, the entry editor and display drivers, and the rail.
It also holds the two pickers and the repertoire Conduct.
The catalog, the entry column, the Situation reading and its editing are the sub-drivers `QAtlas`, `QOccurrence`, `QVignette` and `QScenario`.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.
The constructor takes the veneer's page, which the window pulls by contract ID.
The page's local styles are handed to the look sheet, which otherwise knows only the application's.
It builds the rail with Fresh and Portrait shown, and the two pickers over their placed controls.
The order picker hangs its menu under the whole `PTier` bar.
It builds the four sub-drivers over the same page, each finding its own parts.
It adds the print and export command bindings, which the rail's buttons reach, and subscribes the rail's four notices.
Each named part of the page is pulled through `QContract.QContractFind` by its contract ID.
`_cRepertoire` holds the atlas, the occurrence list, the playwright, the session and the panel's mode.
The atlas's vista carries the order, the inquest and the languages hidden from the entry column.
The panel keeps no copy of any of them and asks the Conduct for each where it needs it.
It is null until `QRepertoireIntroduce` builds it, so only the print and portrait gates guard against that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.
`QRepertoireIntroduce` builds the repertoire Conduct, which builds its editor.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The display view builds the lectern over the editor's display, which Conduct attached to the occurrence panel.
It keeps the atelier and envoy, which the catalog load runs through.
It introduces `QScenario`, `QVignette`, `QAtlas` and `QOccurrence`, which take the window's atelier, never the window.
It attaches the entry display and editor to the same atelier and facets, so an Entry is read and written.
The engine's change notices drive the mode.
Its failures reach the window through the envoy, which the Conduct asks directly.
A dropped inquest rebuilds the language menu through `QRepertoireClearRefine`, while `QAtlas` empties the box.
A workspace change reloads the flags.
It hands the rail the window's navigation and itself as the chronicle host.
It hands both pickers the atlas's aperture, and the order picker the Conduct's fixed list of orderings.
The window fills the situation catalog when the workspace opens, and `QOccurrence` fills the entry column on the same notice.
Every change after that arrives as an announcement.
So the panel is current whether or not its tab is in front.
`QRepertoireVistaRefine` answers the workspace opening, after the Conduct has started the fresh vistas and attached their observers.
The two pickers tick the ordering from the atlas and draw the filter mark.
The flags are loaded before any row is built.
The filter picker then builds its language menu from the languages that load answers.
The catalog is then listed once through `QAtlas`.
The Conduct carried any search text held into the fresh vistas, so the boxes need no re-send.
Its one request is `CRepertoireRowsLoad`, which runs the flag fill and then answers the rows it paints.
`QRepertoireExitRefine` is the Veneer half of the window's exit, since the editor's stop and the playback cancel run in Conduct.
It releases the editor's recording player and closes both pickers' menus, so neither outlives the window.
Undo steps whichever draft is in front one snapshot back, through the Conduct, and redo steps it forward.
The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.
The draft bulletin the engine raises brings the fields back through the ordinary restore.
`QRepertoireShownCheck` says whether the panel is on screen, so the engine knows when a notice needs painting.
`QRepertoireModeRefine` paints the mode the engine decides.
`QScenarioVisibleRefine` lights the writing side, and `QVignetteVisibleRefine` the reading side.
The rail takes the scribe verdict, the mode and bin enablement, and the store verdict.
It ends by refreshing the rail's undo and redo.
`QRepertoireChronicleRefine` lights the rail's chronicle buttons only when the draft in front has a step to walk.
The Conduct reads the entry editor's chronicle while `PEditor` is in front, and the held Situation's otherwise.
`QRepertoireClearRefine` has the filter picker rebuild its language menu after an arrival dropped the query on the vista.
The menu is read back from the vista's cleared filter, and the filter mark follows.
`QRepertoireWorkspaceRefine` reloads the flags, since they do not belong to the old folder's rows.
The Conduct has already closed the shown Situation and dropped the old selection.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.
The rail's Fresh is answered by the Conduct, which decides what is made.
With no Situation chosen and no Entry shown, it opens the editor on a Situation nothing has stored yet.
With a Situation chosen, or an Entry shown, it starts a new Entry carrying that Situation instead.
The rail's delete goes to the Conduct, which acts only while a Situation and not an Entry is shown.
The envoy asks the user first, and a Situation something references is asked about with its usage count.
The rail's save stands for whichever editor is in front.
The Conduct saves an open Entry through the entry editor, and otherwise commits the held Situation.
The rail's toggle asks for the side it names, on whichever side the Conduct stands.
The Conduct asks before leaving an editor, so unsaved wording is never lost silently.
`QRepertoirePressRefine` says the print button is live while an entry or a situation is read.
It answers no before attach, because the command binding exists from the constructor on.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.
`QRepertoirePressObserve` prints the entry being read, or else the situation being read, as the engine portrays it.
The gate asks for the ticket through the envoy and words the page through the engine.
The Conduct picks the page from the side it shows, and the engine builds it from stored rows.
Nothing is read back from the screen.
`QRepertoirePortraitRefine` says the export button is live exactly when an entry is read in the display.
It answers no before attach, as the print check does.
Print may also act on the other page this panel reads, but export acts on entries alone.
`QRepertoirePortraitObserve` exports the entry being read, asking for the file and the format through the envoy.
The engine writes the document from stored rows, and nothing is read back from the screen.

## `QAtlas`

The left column of the Repertoire panel, with the Situation search field over it.
It lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown, and the middle column narrows to the entries referencing it.
The constructor finds `PInquest`, `PAtlas` and `PAtlasEmpty` by contract ID, sets the search hint and subscribes the search field.
`QAtlasIntroduce` is called once the repertoire Conduct exists.
It subscribes the atlas's rows event and the repertoire's dropped search, binds the list and attaches its row fill.
Each keystroke hands the search text to the vista, whose announcement refills the catalog.
`QInquestRefine` empties the inquest box after an arrival dropped the query on the vista.
The owner's `QRepertoireClearRefine` rebuilds the filter menu on the same notice.
`QAtlasRefine` refills the catalog with the ready rows the Conduct reads, already matched and ordered.
The Conduct words the unknown and untitled texts, and reports its own read failure, answering no rows.
The rows arrive as `CCatalogSituation` shapes, the kind and the count carrying their own text.
The Conduct read drops a shown selection whose row no longer stands, and keeps one an open editor holds.
Its clear announces fresh rows that list the same Situations, so the nested refill is harmless.
The tally chips and the entry column follow through the atlas's row notice, so neither is painted here.
The form taking rows paints what the area answered ready, so the paint asks Conduct nothing.
The parameterless form reads them, and the owner's flag-fill Refine hands in what its load answered.
A clicked row asks the Conduct to select its Situation.
The Conduct asks about unsaved work first, and records the station only when the move goes ahead.
`QAtlasItemRefine` fills one row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The kind is written as it is, and the look sheet collapses it while empty.

## `QAtlasItem`

Presentation item for one Situation row in `QAtlas`.
It carries the title, the kind and the usage count, and the Situation id the row loads through.
The id is identity and never displayed.
Two Situations may carry the same title, so a row is never found by what it reads.
A title standing empty is not one thing.
It may never have been written, or it may have been written and be unknown now.
The row is given the mark for the second so the two stay distinct in the catalog.
It is given the untitled text for the first so no row stands blank beside the next.
The constructor builds the row from the found Situation, whose kind and count already arrive worded.
The engine already named an untitled row and an unknown kind, so the row only copies the text.
The chosen mark comes as its own parameter, so the mark has one kind of writer.
`QAtlasItemCount` is how many Meanings and Collocations reference the Situation.
It is the figure that decides whether a delete is legal.
So the catalog carries it and not only the display.
`QAtlasItemChosen` marks the row the panel stands on, painted as an accent edge.
It is the only value that changes after the row is built, and `QSplice` moves the mark in place.
So the list keeps its scroll position.
`QAtlasItemMatch` compares two rows' values, the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.
`QAtlasItemSync` copies the chosen mark onto the held row that `QSplice` kept.

## Stamp

Commit: `c2f335c331fe`
- `QAtlas.cs` `2ff8bdece4ed8a5b`
- `QAtlasItem.cs` `593d5537a55622fd`
- `QRepertoire.cs` `e3253b9e05bcee03`
