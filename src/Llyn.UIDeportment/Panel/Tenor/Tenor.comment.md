# Tenor

The drivers of the tenor panel, which browses the workspace by the Registers its cards carry.
It is the taxonomy panel's shape read through a different question, so it holds the same three columns.
It stands apart because a Register belongs to no language and offers a third ordering.

## `QTenor`

Drives the tenor panel, which owns a reader and an editor over one Entry.
It answers the engine rather than its own visibility.
The panel itself is the veneer's `PTenor` page, which the window places.
The register catalog is painted here, and the entry list beside it by its own driver `QCohort`.
The print and export command bindings sit on the page, so the rail's buttons inside it reach them.
The ordering picker's menu hangs under the whole register search bar.
Each named part is pulled from the page by its contract ID on every read.

`QTenorIntroduce` puts the panel to work through the Conduct tenor panel, which builds its own editor.
Only the medium knows its dispatcher, so the marshal for the area's notices is built here.
The picker and the filter get the register aperture, titled `Degree` and `Grille`.
The ordering menu is built once, with the third ordering that only a register catalog offers.
The entry list driver is introduced with the area, so it subscribes its own rows and opening.
The register catalog gets its row fill here, where its source is set.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.

`QTenorExitRefine` releases the editor's player, so none outlives the window.
It calls no gate.
The window's exit gate `CAtelierClose` closes the editor and stops its playback in Conduct.

`QTenorPressRefine` keeps print and export live exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The buttons follow this answer on their own, so no panel state has to switch them.
Print and export let the engine build the document from the vista and stored rows.
Nothing is read back from the screen.
The export gate asks for the file and the format through the envoy.

`QTenorStoreRefine` enables store only while the editor's draft can be stored.
`QTenorChronicleRefine` lights undo and redo only while the editor has a step to walk.
`QTenorShownCheck` lets a bulletin be acted on only while the page is shown.
`QTenorModeRefine` paints the mode from the shared panel state.
No control's visibility stands in for the mode.

`_cTenor` is the panel's Conduct, holding the register vista and the cohort vista it restored.
The vista carries the order, the query, the chosen Register, and the languages hidden from its entries.
The panel keeps no copy of any of the four and asks Conduct for each where it needs it.
The Register is held by its id rather than its name, since a name may be rewritten under the panel.
A null Register is not an absence to be corrected.
It is the whole workspace, which is what the panel shows first.
The Conduct tenor is null until the window introduces the panel, so only the print gate checks for it.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

`QTenorWorkspaceRefine` answers the area's workspace event.
The area has already closed the entry, cleared the chosen Register and raised the rows.
It loads the flags of the workspace that moved.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

`QTenorVistaRefine` answers the workspace opening, after `CTenor` has restored its vistas.
The picker's checked row and the filter mark are drawn from the vista first.
The flags load before any row is built.
The filter's language menu is then built from the languages that load answers.
The catalog is then listed once, so a row keeps the flag it was built with.
Its read raises the entry rows, so the entry list is painted after the flags too.
Its one request is `CTenorRowsLoad`, which runs the flag fill and then answers the rows it paints.

`QTenorFreshObserve` makes a Register while none is chosen and an Entry under the chosen Register otherwise.
The one fresh gate decides, and it asks the leave question and any wording through the envoy.
`QTenorScribeObserve` swaps the read view and the editor through the shared panel.

`QGamutRefine` lists the Registers the area reads, already counted and in the vista's ordering.
The chosen Register is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A failed read has already been shown by the area, which then answers no rows.
A read that succeeds makes the area raise the entry rows, so the entry list follows.
The form taking rows paints what a flag-fill load answered, so the paint asks Conduct nothing.

`QGamutRegisterRefine` answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied both queries, so the register field only shows it.
`QCohort` empties the entry field from the same event.
The field's own handler still hears the change, and its gate finds the query already empty.
The area raises the rows right after, so the catalog is rebuilt without the view asking.

`QGamutObserve` hands a clicked row's Register id to the toggle gate and ignores a click carrying no item.
The gate records the station, toggles and raises the rows, so clicking the chosen Register lets go of it.
That puts the panel back on the whole workspace without a separate control saying so.

`QGamutItemRefine` fills one register row from its item.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The chosen Register stays visible while the eye is on the Entries beside it.
Without it the entry list would show a filtered set with nothing on screen saying which filter.

## `QCohort`

The entry list of the tenor panel, with the entry search field over it.
It shows the Entries carrying the chosen Register, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.
It subscribes what it paints itself, so the owner `QTenor` only builds and introduces it.
Each keystroke hands the entry search text to the cohort's query gate.

`QQuestRefine` answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied the entry query, so the field only shows it.
The field's own handler still hears the change, and its gate finds the query already empty.

`QCohortRefine` answers the panel's rows event, which the area raises after a successful Register read.
The Entries are never refilled on their own, because the chosen Register may have just changed or vanished.
The engine is handed the register vista and the entry vista, and it decides what they list.
A failed read has already been shown by the area, which then answers no rows.
Conduct picks the empty line's key from whether the entry search holds text.

`QCohortObserve` hands a clicked row's entry id raw to the row gate, or null when it carries no item.
The gate ignores null, asks the leave question and opens the row.
This panel attaches no station, so the gate records none.

`QCohortItemRefine` fills one entry row from its item.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space.

## `QCohortItem`

One Entry as a row of the tenor panel's entry list.
It mirrors the taxonomy panel's row, because both lists answer the same question of the workspace.
That question is which Entries stand under the thing chosen in the catalog beside them.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
A null epithet is stored as empty, so `QCohortItemMatch` compares strings only.
`QCohortItemEpithet` is empty when the setting is off or the entry keeps none.
`QCohortItemMatch` compares rows with the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.
`QCohortItemSync` copies the chosen mark onto the kept row.
`QCohortItemChosen` is the only value that announces a change, because `QCohortItemSync` sets it on a row already shown.
The kept row then repaints its mark without being rebuilt.

## `QGamutItem`

One Register as a row of the tenor panel's catalog.
It carries the id the panel browses by, which is never the name, because a Register may be renamed.
It carries no language, because a Register belongs to none.
The count of cards marked with it is held as a number and shown as text.
Every find rebuilds the rows, so no field changes after the row is made.
The icon key arrives chosen by Core, so the item only resolves it into an image.
`QGamutItemChosen` is whether the catalog's vista has this row chosen, which the row template tints.

## Stamp

Commit: `c05429890cec`
- `QCohort.cs` `65abdd94a738e6b8`
- `QCohortItem.cs` `f9097ed5bf415e27`
- `QGamutItem.cs` `1bc14a0eef461532`
- `QTenor.cs` `04047280260bb76a`
