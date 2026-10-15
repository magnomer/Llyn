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
The stencil clerk relays reach its list read, field edits and media move.
The line relay reaches the cue clerk's listing read.
The schedule relay builds a default deck schedule.
The stencil clerk is built over the same clerks the entry relay builds.
The deck and cue clerks are built over the test rig, as the stencil clerk is.
The cue clerk takes the same clerks the stencil clerk takes, for its listing line.
The resolve relay crosses here with a stored cue, a template and the portrait label.
The deck outlet is built over an engine, and its deck, folder, cue and template calls pass through unchanged.
So a gate test prepares its decks through the deck port without asking the user.
The depot relays build the folder adapter, relay its digest read and drive its save with no cancellation.
The sheet relay builds a sheet from settings, a header and rows, so a test writes cells inline.
The manifest file is built over a keep file in a given folder, and its read and save are relayed.
A manifest is built from a deck map as given, and a save on any manifest store is relayed too.
A fresh latch is built here, so a test holds one without constructing it.
The latch relay reads one engine's latch from its workspace staff.
The latch's start, finish and check are relayed as they are.
The depot rig relay swaps the depot and the manifest of a real rig for fakes.
So an export test reads real decks and cues and writes nowhere.
The depot clerk is built with a fresh gate, a silent fault sink and the phone folder `Phone`.
Its export runs with the default portrait label and no cancellation.

## `TDepotFake`

An in-memory phone folder, copying `TOutpostFake`.
It holds one folder, so it keys files and media by deck code alone.
Each save stores a fresh digest, so a test can fake a phone export by changing it.
Copies and deletes are recorded by name, so a test can count them.
A media name comes from a map of source paths, so two paths can share one content name.

## `TDeckManifestFake`

An in-memory deck manifest that starts empty.
A save replaces the stored manifest whole, as the file store does.

## Stamp

Commit: `2f9a06b8eb95`
- `TDeckManifestFake.cs` `3fab1686919043f2`
- `TDepotFake.cs` `af48a461923f322d`
- `TInterfaceRehearsal.cs` `061d0df9405facca`
