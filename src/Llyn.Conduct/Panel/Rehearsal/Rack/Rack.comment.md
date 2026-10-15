# Rack

This folder holds the deck gates: the deck list with its folders, and the deck editor with its bearings.

## `CDeck`

The deck list gates, one sealed class owning the chosen deck.
It registers the `Deck` tab, whose station is the chosen deck and whose arrival chooses it.
It builds `CDeckCharter`, the editor of the chosen deck, as `CGuild` builds its desk.
  The chosen deck has one owner, so the list opens, clears and guards the editor.
It builds `CDeckCue`, `CDeckStencil` and `CDeckAccession` too, handing each a read of the chosen deck.
The tab's leave question is the editor's, so unsaved deck settings ask before the tab changes.
`CDeckSelect` and `CDeckCreate` ask it first, as `CGuildAuthorSelect` does.
  A declined question keeps the old deck chosen.
A workspace open clears the chosen deck, because each workspace keeps its own decks.
`CDeckChanged` tells the driver to read the roll again after any change.
`CDeckViewSet` keeps which of the three views the Deck tab shows, and ignores a number outside `CDeckView`.
  It raises `CDeckChanged`, and the first view is the adder.
The engine raises no deck bulletin, so the gate's own changes are the only ones.

## `CDeck` gates

Each gate makes its one engine call and passes the engine's answer through.
Order, folder and grain rules live in the engine, so a gate checks none of them.
A refusal or a failure reaches the user through `LLedgerFailureShow`.
  A list read failure shows `Deck.LoadFailed`, and a change failure shows `Deck.SaveFailed`.
`CDeckCreate` creates with the grain the deck list keeps, which starts at the first creatable grain.
  `CDeckGrainSelect` keeps the grain at an index and ignores an index out of range.
  A grain the store does not hold fails in the engine and is shown.
A created deck becomes the chosen one.
`CDeckFolderCreate` and `CDeckCreate` ask the name through the coinage question, and a declined name creates nothing.
`CDeckFolderDelete` asks nothing, because its decks stay and move to the top level.
`CDeckGrainRead` lists the creatable grains, each with its wording key and a flag for the kept one.
  `CDeckRoll` carries the same list.
  `Free` is left out, because the store takes grains one to five only.
  The driver binds the list and passes the chosen index back unread.
`CDeckDestinationRead` lists where a deck can move: every place but its own, the top level first.
  The top level has the key `Deck.MoveTop`, and a folder has its name.
  A deck at the top level is offered folders only.
  An unknown deck is offered no destinations.
  `CDeckMove` takes one destination, so the driver decides nothing.
`CDeckOrderMove` moves one deck by an offset among the decks of its own folder.
  It reads the roll first, so the new order comes from the engine's order.
  An offset past either end moves nothing.
  So the driver sends only up or down.
`CDeckDelete` reads the deck's cues first, so the question shows their count.
  Deleting the chosen deck clears the chosen deck and the editor.
  That read is a second engine call, kept because the count must come from the engine.
  An empty deck gets the plain question, as `CEtymology` asks it.

## `CCharter`

The deck editor, one sealed class owning the stored deck and the draft.
Both hold the deck and its bearings as the engine read them.
Each setter replaces the draft whole, so the draft has one writer at a time.
The change check compares the draft with the stored pair at read time.
  So no dirty flag can disagree with the values.
A setter with no deck open does nothing, as `CGuildNameSet` does.
`CCharterRead` returns the draft as `CCharterState`, or null while no deck is open.
  An absent draft reads as absent, so the driver never sees made-up values.
  The read changes nothing, so the change check still compares the draft with the stored pair.

## `CCharter` gates

Each gate takes the one raw value the user changed.
  Conduct builds the deck and bearing records, so the driver never sends back values it read.
A choice gate takes the box's place and adds one, as the markup lists choices in Core order.
  A place below zero means no pick, and the gate does nothing.
A number gate takes the field's text.
  Text that does not read as a whole number keeps the value the draft holds.
  An empty quorum stores null, and an unreadable one with no value held stores zero.
No gate checks a schedule or a code.
`CCharterSave` makes the one engine call, `LEngineDeckUpdate`, with the deck and its bearings.
  The engine refuses a taken code with `Refusal.DeckCodeTaken`.
  `LLedgerFailureShow` shows that refusal, and any failure, under `Deck.SaveFailed`.
`CCharterSave` returns false after a failure, so the leave question can stay.
A saved draft becomes the stored pair without a second read.
  The engine trims and normalizes the code, so the draft code may differ until the next open.
The editor has no grain setter, because a deck's grain is fixed at creation.
  The engine's update keeps the stored grain.
A side gate takes the direction and the box's place, and flips that bit in the face.
`CCharterStatisticsSet` takes the direction and the text, read as the number gates read it.
A bearing gate for a direction the deck lacks does nothing.
The schedule target counts hours, as `LDeckScheduleTarget` does.

## Leave question

The editor asks through `CEnvoyLeaveConfirm` only when the draft differs from the stored pair.
A cancelled question keeps the user in place.
  A refused save keeps the user in place too.
  A declined save drops the draft back to the stored pair.
The editor joins the workspace drafts, as `CGuild` does, so a workspace change or a quit asks too.
The language-role warning is left out until its rule is decided.

## `CCharterState`

The deck editor's draft as the driver shows it.
The lead and order are the choice places the gates take, so the driver adds nothing.
Each other number is the engine's number, and the basic flag and the code are the draft's own.
Each bearing is a `CCharterBearing` row, in the engine's direction order.

## `CCharterBearing`

One direction row of the deck editor.
It carries the direction, the front and back faces, the statistics and a caption key.
  The key is `Bearing.Forward` for direction one and `Bearing.Backward` for direction two.
  So the driver only looks the key up.
An unknown direction throws, because the store allows only these two.

## `CDeckRoll`

The deck list as the driver shows it, in the engine's order.
It is empty when no deck exists, whatever folders stand, and when the list read fails.
It also carries the creatable grains, as `CDeckGrainRead` lists them.

## `CDeckRow`

One deck as Conduct shows it.
Its grain shows through the stencil state, so the row carries none.

## `CDeckGrain`

One creatable grain by its wording key.
It marks whether it is the grain the deck list keeps.

## `CDeckDestination`

One place a deck can move to, for the deck it was read for.
The folder is null for the top level, which carries a wording key instead of a name.

## `CDeckView`

The three views of the Deck tab: the adder, the stencil and the editor.
The adder is the default.

## `CDeckFolder`

One deck folder as Conduct shows it.
It is not named `CDeckFolderRow`, because its members would pass three name components.

## Stamp

Commit: `2f9a06b8eb95`
- `CCharter.cs` `e3365847c7dc131f`
- `CCharterBearing.cs` `e09b60c3f229adea`
- `CCharterState.cs` `bec7a3928d880965`
- `CDeck.cs` `8129f8d72b6419ba`
- `CDeckDestination.cs` `691c943e20e8ea03`
- `CDeckFolder.cs` `b97e5bf3a85d3d30`
- `CDeckGrain.cs` `2aeff1e5c9946d85`
- `CDeckRoll.cs` `a1b714825a072391`
- `CDeckRow.cs` `bb67344dff6ea3b0`
- `CDeckView.cs` `cb71a6331ebd550f`
