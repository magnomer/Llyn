# Rehearsal

This folder holds the relays for flashcard decks.
Each realm owns its relay file, so later deck jobs add their relays here.

## `TInterfaceRehearsal.cs`

The flashcard half of the test interface.
Tests never call a production rule, so every deck rule crosses here.
The card relay fills the fields no orphan rule reads with fixed values.
So a test names only the grain and the references.
The deck, cue and stencil stores are opened as their adapters and driven through their ports.
The card relay doubles as the cue fixture, since it already fills every field.
The fixture builders fill a deck, stencil or bearing with plain defaults.
The cue fixture builds numbered cells of one column, each valued by its column and index.
So an archive test names only what it checks.
The slot fixture builds a template of one slot at index one, so a face test names only the body.
Its three-part overload builds one slot alone, and the cell relay builds one cell alone, each as given.
The default relay reads a grain's default template through the Core rule.
The body relays reach the Core rules that edit and check a slot body.
The face relays build a face from cells and split one side of it into an `LCueSide`.
The stencil clerk relays reach its list read, field edits and media move.
The line relay reaches the cue clerk's listing read.
The study relays reach the stencil clerk's face read and the cue clerk's active read.
The deck port's face and active reads are relayed too.
The schedule relay builds a default deck schedule.
The stencil clerk is built over the same clerks the entry relay builds.
The deck and cue clerks are built over the test rig, as the stencil clerk is.
The cue clerk takes the same clerks the stencil clerk takes, for its listing line.
The resolve relay crosses here with a stored cue, a template and the portrait label.
The deck outlet is built over an engine, and its deck, folder, cue and template calls pass through unchanged.
So a gate test prepares its decks through the deck port without asking the user.
The locker relay builds the locker entrance over a keep file in a given folder and a `TClockFake`.
The shipment relay builds a shipment for deck 1 from cells.
It numbers the cues from 1, so a test writes cells inline.
Its typed rows hold the trimmed value of each picture or sound cell that is a full path.
They hold null elsewhere.
The export relay drives any depot with a silent fault sink and no cancellation.
A fresh latch is built here, so a test holds one without constructing it.
The latch relay reads one engine's latch from its workspace staff.
The latch's start, finish and check are relayed as they are.
The depot rig relay swaps the depot of a real rig for a fake.
So an export test reads real decks and cues and writes nowhere.
The depot clerk is built with a fresh gate, a silent fault sink and the phone folder `Phone`.
Its export runs with the default portrait label and no cancellation.

## `TInterfaceScoreboard.cs`

The statistics half of the test interface.
It relays the census and week rules, the clerk's census read and the deck port's census read.
It stands apart from `TInterfaceRehearsal.cs` because the statistics tab is its own panel.

## `TDepotFake`

A depot that writes nowhere, copying `TOutpostFake`.
It records each shipment, so a clerk test reads what the clerk resolved and sent.
Each export answers the receipt the test set, empty by default.

## Stamp

Commit: `1f3467bbe5c6`
- `TDepotFake.cs` `586b148721938e7f`
- `TInterfaceRehearsal.cs` `efc086788daa0e6e`
- `TInterfaceScoreboard.cs` `0b65d5a981d9ea08`
