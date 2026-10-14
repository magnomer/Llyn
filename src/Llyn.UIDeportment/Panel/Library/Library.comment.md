# Library

The library panel and the duplex panel, with the entry list and the wing both share.
It stands apart because these panels are medium: they own the match lists, keys and focus.
The vista, rows and saved standing live in Conduct's `CLibrary` and `CWing`.
Every branch a driver once carried lives in Conduct, so a driver forwards what the page carries.

## `QLibrary`

Drives the library panel by what it is made of and what it forwards.
The search, the index, the read-only display, the editor and the import are wired here.
The ordering, the filter and the command row each have their own shared driver.
The panel itself is the veneer's `PLibrary` page, which the window places.
Each named part is pulled from the page by its contract ID on every read.
The constructor adds the print and export command bindings to the page.
The rail's buttons sit inside the page, so their commands reach these bindings.
It hands `PLibraryRail` to a `QPanelRail`, `PLibraryOrder` to a `QChoiceOrder` and `PLibraryFilter` to a `QChoiceFilter`.
The order menu hangs under the whole `POrder` bar.
The import button is the rail's collapsed `PPanelRailMarkup`, found inside `PLibraryRail`.
Only this panel imports, so it shows the button and sets its icon and click here.
`QLibraryIntroduce` puts the panel to work through the Conduct `CLibrary`, which it builds over the atelier.
It keeps the atelier and envoy for the catalog load, and hands the volume and mention menu on.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The Conduct builds its own entry editor, which this panel wraps for the editor view and the lectern.
The display view builds the lectern over the editor's display, which Conduct attached to the panel.
A stored Mention's sense, raised by `CMentionSenseChosen`, scrolls into view on that lectern's compass.
The navigation's entry open lands in the library, so this display is the one showing the entry.
The picker and the filter get the panel's aperture, titled `Order` and `Sieve`.
The ordering menu is built once, from the orderings `CLibrary` offers.
It subscribes the area's events and reads nothing yet.
`QLibraryVistaRefine` fills it when the workspace opens, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The print and portrait command checks answer false until the controller exists.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.
`QLibraryIndexIntroduce` builds the entry list over the page's controls and refills it on each announcement of new rows.
The list is medium, so it lives here and not in Conduct.
`QLibraryIndexRefine` hands the rows to the list with the library's empty verdict.
Conduct counts the rows, so the empty notice needs no count kept here.
The form taking rows paints what the area answered ready, so the paint asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.
`QLibraryStoreRefine` enables the rail's store button only while the editor's draft can be stored.
`QLibraryVistaRefine` answers the opening of a workspace, after the atelier has started the tab's vistas.
It paints the order and the filter mark, then waits for the flags.
The flags are loaded before the first rows are built, because a row reads its flag at construction.
It builds the filter menu from the languages the load answers, then reads the rows once.
Its one request is `CLibraryRowsLoad`, which runs the flag fill and then answers the rows it paints.
The observers that carry each announcement live in `CLibrary`, so this driver holds none.
`QLibraryWorkspaceRefine` loads the flags of the new workspace, the entry already closed in Conduct.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.
`QLibraryShownCheck` is the shown seam, which only the page can answer.
`QLibraryExitRefine` releases the editor's recording player when the window closes.
The editor and the display stop through `CLibrary`'s closure, which the atelier's exit gate runs.
`QLibraryModeRefine` writes the mode and enablement the controller holds into the editor, the display and the rail.
The rail folds its own button pairs from the scribe flag it is handed.
`QLibraryIndexObserve` puts the panel on the clicked row's entry, and the gate records the station first.
The click hands over the row's item id, so no control decides the request.
A sender without a row selects none, which leaves the panel on no entry.
`QLibraryRowRefine` fills one row through the shared index fill, then subscribes its click once.
The click is removed first, so a refill never doubles it.
The search field hands its text to the query gate.
The rail's new-record notice asks the panel for a fresh entry.
The rail's mode toggle goes to the scribe gate, true for the editor and false for the read view.
The store notice goes to the editor's save gate, and the delete notice to the panel's delete gate.
`QLibraryMarkupObserve` calls `CLibraryMarkupImport`, which asks every question itself.
The gate asks for the file, then the customs question, and shows the report through the envoy.
Import belongs on this panel rather than the editor.
What arrives is any number of entries, none of them the one being written.
`QLibraryPressRefine` says the print button is live exactly when an entry is read in the display.
The controller answers, so no control state is read.
`QLibraryPressObserve` prints the entry being read, as the engine portrays it.
The gate asks for the ticket through the envoy and names the vista.
Nothing is read back from the screen.
`QLibraryPortraitObserve` exports the entry being read the same way, asking for the file and the format.
The engine writes the document from stored rows.
`QLibraryChronicleRefine` lights the rail's two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `QDuplex`

Drives the duplex panel by building its two wings and putting them to work.
Searching, picking, reading and restoring live in the wing, so this file only introduces both.
The panel itself is the veneer's `PDuplex` page, which the window places.
The page places the veneer's `PWing` twice, and each place gets its own wing driver.
`QDuplexIntroduce` puts both wings to work on the window's facets, telling each whether it is the left place.
Each wing subscribes for itself, so the panel subscribes to nothing.

## `QWing`

Drives one side of the duplex panel, made of a search bar, its matches and the Entry picked from them.
The two sides are peers, so each place of the veneer's `PWing` gets its own driver, vista and display.
The panel never writes, so there is no editor and nothing to discard.
The match list, the field's keys and focus live here, since they are medium.
Each named part is pulled from the place by its contract ID on every read.
The constructor builds the match list over the place's list and empty notice, since both are there from the start.
It ties both droppers to their popups, sets both icons, and subscribes the field and list events.
It attaches the index row fill and watches the list's visibility for the tray.
`QWingIntroduce` builds Conduct's side over the atelier and envoy.
It keeps both for the catalog load, and hands the volume and mention menu to the display.
`left` tells Conduct which place this is, so the side picks its own tab and saved entry.
The side's announcements re-list the matches on the list's thread.
A loaded entry re-lists the matches, and the display's area opens it on the lectern itself.
A key move re-lists the matches on the spot, since it is heard on the list's thread.
The display is its own subscriber, so it stays current on its own.
`QWingRefine` answers every workspace open, once Conduct has handed the side its vista.
The list is cleared, then the flags are loaded before any row is built.
The language menu is built from the languages that load answers.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.
The dropdown lists the shared entry orderings, and the filter mark is drawn from the side's verdict.
The field is emptied, since the fresh vista holds no query.
Conduct restores the entry itself, so this paint calls no gate.
`QWingSieveBuild` builds the language sieve from the flag load, as the library does.
`QWingIndexRefine` lists the matches, already filtered, sorted, numbered and marked by the engine.
The empty notice shows as `CWingEmpty` answers.
It runs on each announcement, after each entry the side loaded, and after each key move.
`QWingSieveRefine` shows the mark on the sieve button while the vista hides any language.
A clicked order row sets the ordering its tag carries, then closes the dropdown.
The announcement then re-lists the matches.
A clicked language row sets the filter its list now stands for, then redraws the mark at once.
`QWingQueryObserve` hands the text to the vista.
Then it shows the list as `CWingRowsShown` answers, which holds while the vista holds a query.
`QWingKeyRefine` is the field's first key handler.
Escape asks `CWingRowsClose` and takes the key only when the list was open.
A handled key ends the route, so the key observe never hears it.
`QWingKeyObserve` is the keyboard path from the field into the open list, heard after the key refine.
A closed list lets every key through to the field, since the gates refuse while the list is hidden.
Conduct holds whether the list is shown, since a control's state never gates a request.
Each key picks one gate, and the gate answers whether it took the key.
Enter hands the chosen row to `CWingRowsOpen`, and the index follows `CWingRowsShown` afterwards.
Down and Up hand the direction to `CWingRowMove`, and the answered entry is scrolled into view.
The gate re-marks the rows through `CWingRowsChanged`.
Which key means which action is the medium's own routing, so it stays here.
`QWingLookupObserve` closes the list when keyboard focus leaves the whole lookup grid.
Focus moving between the field and the list stays inside the grid, so a clicked row is never a leave.
A clicked row hands its id to `CWingRowsOpen`, and the index follows `CWingRowsShown`.
`QWingRowRefine` fills one result row through the shared index fill, then subscribes its click once.
`QWingTrayRefine` copies the list's visibility onto the seam and the tray, the work their bindings did before.

## `QIndex`

The entry list of the Library panel and of each duplex wing.
It holds the rows and the empty notice.
The panel hands over the veneer's controls once, and fills and steers them through it from then on.
It is medium, so it lives beside the views and never in a controller.
The view control is touched only by the index, so no caller reads a control's state.
The constructor sets the held list as the view's source once.
Every later fill splices that list, so the view's source never changes and its rows keep their containers.
`QIndexShownRefine` shows or collapses the list as Conduct says, and the index keeps no shown flag.
`QIndexRefine` splices the fresh rows into the held list, so unchanged rows keep their containers and the scroll position.
The empty notice shows as the panel's Conduct says.
`QIndexClearRefine` empties the list without touching the empty notice.
`QIndexChosenRead` answers the entry of the row the engine marked chosen, or null when no listed row is.
A choice made under an earlier query may name an entry the rows no longer hold, and reads null.
`QIndexScrollRefine` brings the row of an id into view, when its container is built.
A null id scrolls nothing, since the gate answered no row.

## `QIndexItem`

Presentation item for one entry row in the Library and Duplex entry lists.
It carries the headword and language the row shows, and the entry id the row loads through.
The id is identity and never displayed.
So the row stays selectable after a headword is edited into something another entry reads as.
The flag is resolved once for the language and handed to the row, not read from disk by the row.
A row is built while its list is being filled, and reading a file there would stall the fill.
`QIndexItemChosen` marks the row the panel stands on, painted as an accent edge.
It is the only value that changes after the row is built, and `QSplice` moves the mark in place.
So the list keeps its scroll position.
`QIndexItemName` is the headword, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`QIndexItemEpithet` prints after the headword, small and muted, in the reading the language pack names.
It is empty when the setting is off or the entry keeps none.
`QIndexItemBuild` makes one item per row shape, in the order the engine returned them.
Each flag comes from `QEnsignImage`, which has drawn it before the rows are asked.
`QIndexItemMatch` compares two rows' values, the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.
`QIndexItemSync` copies the chosen mark onto the kept row and raises only when it moved.
So untouched rows repaint nothing.
`QIndexItemRefine` fills one index row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The library index and each duplex wing share this fill, and each subscribes its own click after it.

## `QMarkup`

The file dialog that answers the library import's file question for the envoy.
It owns no state and opens no file, because the workspace is the engine's to read.
`QMarkupConsult` answers the file the reader picked, or null for a cancelled pick.
A cancelled pick is not a failure and leaves the workspace exactly as it was.
The format's own extension is offered first, with everything else still reachable.
Markup is plain text, so a file that carries it under another name is still importable.

## Inline notes

### QLibrary.cs: `_qLibraryEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cLibrary.CLibraryEditor);`

The editor opens on no entry, and this panel puts it on one when the reader asks to write.
The editor deportment was made for this panel, so its held work is told apart from the input panel's.
A store may have changed the headword the index lists and the text the display shows.
The engine announces it through the vista, so both are read again from what was written.
It is the same announcement whether the store happened in this panel's editor or in another tab.

## Stamp

Commit: `c6e60141a172`
- `QDuplex.cs` `ca987539dd4da4cd`
- `QIndex.cs` `c11007a0de35b95b`
- `QIndexItem.cs` `f2e8778b89af3698`
- `QLibrary.cs` `96e599628a33dafe`
- `QMarkup.cs` `2a6d5e1d2cf25221`
- `QWing.cs` `cd859f18f3ed85d6`
