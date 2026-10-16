# Rack

The drivers of the deck list and the deck editor.

## `QDeck`

Drives the deck list on the veneer's `PDeck` page, which the window places.
The driver is split by role into three classes.
  `QDeck` owns the list, its rows, the order buttons, the grain box and the create buttons.
  `QDeckMenu` owns each row's menu, and `QDeckName` owns the inline rename field.
Each named part is pulled from the page by its contract ID on every read.
`QDeckIntroduce` builds `CDeck` and `CDepot`, so the deck tab is registered once.
  It reads no deck list, so the first repaint fills the page.
  It builds `QDeckMenu` with both gates and the one `QDeckName` the driver holds.
  It builds `QCue` on the same page, hands it `CDeckCue` and keeps no field for it.
  It builds `QJotter` on the `PJotter` part and the listing `PCue` the same way, with `CDeckCue`.
  It builds `QCharter` on the `PCharter` part the same way, with `CDeckCharter`.
  It builds `QStencil` on the `PStencil` part the same way, with `CDeckStencil`.
  It builds `QAccession` on the `PAccession` part the same way, with `CDeckAccession`.
  `CDeckChanged`, raised by a deck select among others, also repaints the listing.
  `CAccessionChanged` repaints the listing too, so an added cue shows at once.
The view strip shows the view that `CDeckView` reads, never one the driver remembers.
  `QDeckViewObserve` hands the clicked tab's view to `CDeckViewSet` as its number.
  `QDeckViewRefine` marks that tab and shows its sheet on every `CDeckChanged`.
  The sheet, not the view, takes the visibility, because `QCharter` and `QStencil` set their own.
Every user action hands its raw value to one `CDeck` or `CDepot` gate.
  The driver checks no name, order or grain, and keeps no state beyond the painted list.
`CDeckChanged` repaints the whole list and refills the grain box from one `CDeckRead`.
`CDepotChanged` paints the export line, so the busy line and the receipt come from Conduct.

## Grain box

The box lists `CDeckRollGrains` and selects the grain Conduct marks as chosen.
Its selection hands `SelectedIndex` to `CDeckGrainSelect`, and `CDeck` keeps the grain.
The refill runs with the selection handler detached, so a repaint sends no request.

## Rows

Top-level decks come first, then each folder with its decks, as the engine orders them.
The driver only pairs decks with folders by id.
Folder rows get the folder icon and no order buttons.
Deck rows clear the icon.
A deck row click, and only a deck row click, selects the deck through `CDeckSelect`.
The order buttons send an offset of one up or down to `CDeckOrderMove`.
  Their icon keys are literals, written the same way on every row.
Each row paint hands the row to `QDeckMenuRefine` and `QDeckNameRefine`, so each role wires its own parts.
The top-level new deck button sends no folder to `CDeckCreate`.

## `QDeckMenu`

Each row's menu is built when it opens, so the destinations are read fresh.
A folder row offers a new deck inside it, rename and delete.
A deck row offers rename, move, export and delete.
The export option is greyed while the last `CDepotChanged` state was busy.
Each option is one handler that reads its row through `QSender` and serves one gate.
  The folder rename option hands `CDeckFolderUpdate` to `QDeckNameOpen`.
  The deck rename option hands `CDeckNameSet` to it.
  So the option, not the saved row, chooses the gate.
Its members keep the `QDeck` owner name, as `QSGalleyBar` keeps `QSGalley`.
Move lists `CDeckDestinationRead` for the row, top level included when the deck sits in a folder.
  Each entry carries its destination as a `QDeckDestination`, and its click hands it to `CDeckMove`.

## `QDeckName`

Rename swaps the row text for an inline field in the view, and hides the count text meanwhile.
`QDeckNameOpen` keeps the gate its option handed over until the next open.
  Only an open writes it, and a save is reachable only while a field is open.
Enter or a lost focus saves through that gate.
Escape closes the field and restores the shown name.
  The lost-focus handler is attached only while the field is open.
  Closing detaches it before hiding the field, so Escape and a repaint save nothing.
  A save restores the row text whatever the gate does.

## `QCharter`

Drives the deck editor `PCharter`, copying `QAutograph`.
`CDeckChanged` repaints it from one `CCharterRead`, and a null state collapses it.
  The handlers are detached while it paints, so painting sends no setter.
Every edit hands one raw value to one `CCharter` gate.
  Save, change check and leave question stay in Conduct.
  The save button calls `CCharterSave`.
A direction row is a `QBearingItem` copied from one `CCharterBearing` by name.
  Its caption takes the row's key as a resource reference.
A side box sends its direction and its place in its face, and Conduct flips the bit.
A number field sends its text, and a choice sends its place.
  Conduct reads both, so the driver adds and parses nothing.

## `QBearingItem`

One direction row of the deck editor, as the surface holds it.
It copies the Conduct row by name, so the surface never holds a Conduct type.

## `QDeckItem`

One painted row of the deck list, a deck or a folder.
It holds the count text, formatted from the row's key and count, and empty for a folder.

## `QDeckDestination`

One entry of a deck row's move menu, holding the `CDeckDestination` it was painted from.

## Stamp

Commit: `bd299c638b2c`
- `QBearingItem.cs` `ee8d20be1e44aaf2`
- `QCharter.cs` `3053ed7c05a131dc`
- `QDeck.cs` `0d3ed46758e04153`
- `QDeckDestination.cs` `7f2b9de20e5fe696`
- `QDeckItem.cs` `9e42d942c5b3cbbc`
- `QDeckMenu.cs` `d3b8b6f664d97888`
- `QDeckName.cs` `71b0d77e4eb3043a`
