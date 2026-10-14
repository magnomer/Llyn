# Rehearsal

This folder holds the flashcard gates, starting with the deck list and the deck editor.

## `CDeck`

The deck list gates, one sealed class owning the chosen deck.
It registers the `Deck` tab, whose station is the chosen deck and whose arrival chooses it.
It builds `CDeckCharter`, the editor of the chosen deck, as `CGuild` builds its desk.
  The chosen deck has one owner, so the list opens, clears and guards the editor.
It builds `CDeckCue` and `CDeckStencil` too, handing each a read of the chosen deck.
The tab's leave question is the editor's, so unsaved deck settings ask before the tab changes.
`CDeckSelect` and `CDeckCreate` ask it first, as `CGuildAuthorSelect` does.
  A declined question keeps the old deck chosen.
A workspace open clears the chosen deck, because each workspace keeps its own decks.
`CDeckChanged` tells the driver to read the roll again after any change.
The engine raises no deck bulletin, so the gate's own changes are the only ones.

## `CDeck` gates

Each gate makes its one engine call and passes the engine's answer through.
Order, folder and grain rules live in the engine, so a gate checks none of them.
A refusal or a failure reaches the user through `LLedgerFailureShow`.
  A list read failure shows `Deck.LoadFailed`, and a change failure shows `Deck.SaveFailed`.
`CDeckCreate` takes the grain the user chose, with no default.
  A grain the store does not hold fails in the engine and is shown.
A created deck becomes the chosen one.
`CDeckFolderCreate` and `CDeckCreate` ask the name through the coinage question, and a declined name creates nothing.
`CDeckFolderDelete` asks nothing, because its decks stay and move to the top level.
`CDeckDelete` reads the deck's cues first, so the question shows their count.
  That read is a second engine call, kept because the count must come from the engine.
  An empty deck gets the plain question, as `CEtymology` asks it.

## `CCharter`

The deck editor, one sealed class owning the stored deck and the draft.
Both hold the deck and its bearings as the engine read them.
Each setter replaces the draft whole, so the draft has one writer at a time.
The change check compares the draft with the stored pair at read time.
  So no dirty flag can disagree with the values.
A setter with no deck open does nothing, as `CGuildNameSet` does.

## `CCharter` gates

Each setter casts the driver's number to the Core value and stores it unread.
No setter checks a schedule or a code.
`CCharterSave` makes the one engine call, `LEngineDeckUpdate`, with the deck and its bearings.
  The engine refuses a taken code with `Refusal.DeckCodeTaken`.
  `LLedgerFailureShow` shows that refusal, and any failure, under `Deck.SaveFailed`.
`CCharterSave` returns false after a failure, so the leave question can stay.
A saved draft becomes the stored pair without a second read.
  The engine trims and normalizes the code, so the draft code may differ until the next open.
The editor has no grain setter, because a deck's grain is fixed at creation.
  The engine's update keeps the stored grain.
`CCharterBearingSet` replaces the bearing of that direction, or adds one when the deck has none.
The schedule target counts hours, as `LDeckScheduleTarget` does.

## Leave question

The editor asks through `CEnvoyLeaveConfirm` only when the draft differs from the stored pair.
A cancelled question keeps the user in place.
  A refused save keeps the user in place too.
  A declined save drops the draft back to the stored pair.
The editor joins the workspace drafts, as `CGuild` does, so a workspace change or a quit asks too.
The language-role warning is left out until its rule is decided.

## `CCue`

The flashcard gates of the chosen deck, built by `CDeck` beside the editor.
It reads the chosen deck from `CDeck` at each call and keeps no copy.
  So the chosen deck keeps one writer.
A gate that needs a deck does nothing while none is chosen.
`CCueChanged` tells the driver to read the deck's flashcards again after any change.

## `CCue` gates

Each gate passes the engine's answer through and checks no flashcard rule.
The engine refuses a bound flashcard's cell edit with `Refusal.CueBound`.
  It also owns every column and index limit.
A read failure shows `Deck.LoadFailed`, and a change failure or a refusal shows `Deck.SaveFailed`.
`CCueCreate` makes a free flashcard with no cells.
`CCueRemove` asks `Cue.RemoveConfirm` first, and a declined question removes nothing.
  An empty choice asks nothing.
`CCueCellSet` reads the cells, replaces the one at that column and index, and saves them all.
  That read is a second engine call, kept because the engine's save replaces every cell.
`CCueCellOpen` asks for a file through `CEnvoyMediaRead`, then saves its path as `CCueCellSet` does.
  The column chooses the title key and the suffixes, which are constants in `CCue`.
  Only a picture or a sound column opens a file, so another column throws.
  A declined dialog keeps the cell.

## `CCueCell`

One cell of a flashcard, or one slot of a template, as Conduct shows it.
A slot and a cell share their column, index and text, so one row carries both.
The column is the engine's number, as `CDeckRow` carries the grain.

## `CStencil`

The template editor of the chosen deck, built by `CDeck` beside the editor.
It reads the chosen deck from `CDeck` at each call, as `CCue` does.
Each slot set saves at once, so the editor keeps no draft and asks no leave question.
`CStencilChanged` tells the driver to read the template again, as `CEtymologyChanged` does.
A read failure shows `Deck.LoadFailed`, and a change failure shows `Stencil.SaveFailed`.
`CStencilSave` asks the name through the coinage question under `Coinage.Stencil`.
  A declined name saves nothing.
  The saved template leaves the deck's own template as it was, so no change is raised.
`CStencilApply` copies a saved template's slots into the deck's own template.
  No engine read lists the saved templates yet, so the driver cannot offer them.

## `CStencilState`

The chosen deck's template as the driver shows it.
It is empty while no deck is chosen.

## `CDepot`

The deck export gate, copying `CCourier`.
It takes the deck by id, so it needs no chosen deck and stands apart from `CDeck`.
The busy value is read from the engine's workspace latch at each `CDepotRead`.
  The gate keeps no busy flag, so a run another driver started reads as busy too.
  A second run is refused by the engine with `Refusal.DepotBusy`, and the gate shows it.
The receipt line is the only state the gate owns, and `CDepotExport` is its one writer.
`CDepotChanged` tells the driver to read the state again, as `CCourierChanged` does.

## `CDepot` gates

`CDepotExport` reads the portrait label, then makes its one engine call, `LEngineDeckExport`.
It raises once the export task exists, so the driver reads the latch already started.
Every failure and refusal shows under `Deck.ExportFailed`, as `Courier.SendFailed` does.
  A failed run clears the receipt line, because it has no receipt.
The line formats `Deck.ExportReceipt`, with up to ten failed names under `Deck.Failed`.
  A translation with broken placeholders falls back to four plain numbers.
No busy text exists for the deck, so the line stays empty while a run is under way.

## `CDepotState`

The export state as the driver shows it, with the busy value and the last receipt line.

## `CDeckRoll`

The deck list as the driver shows it, in the engine's order.
It is empty when no deck exists, whatever folders stand.

## `CDeckRow`

One deck as Conduct shows it.
The grain is the engine's number, so no driver needs the Core enum.

## `CDeckFolder`

One deck folder as Conduct shows it.
It is not named `CDeckFolderRow`, because its members would pass three name components.

## Stamp

Commit: `f6dae2392935`
- `CCharter.cs` `2e1557964e1a6f2e`
- `CCue.cs` `03ca507d616cf568`
- `CCueCell.cs` `59241dca6dfe705b`
- `CDeck.cs` `24632dfb4138398d`
- `CDeckFolder.cs` `b97e5bf3a85d3d30`
- `CDeckRoll.cs` `57644ac4584db997`
- `CDeckRow.cs` `94bfc5e033b3d433`
- `CDepot.cs` `8427cd893faa6b46`
- `CDepotState.cs` `c9c1510f8ba2c3b4`
- `CStencil.cs` `cf4f17a45f323ed6`
- `CStencilState.cs` `9eb25fa4a14b7ed6`
