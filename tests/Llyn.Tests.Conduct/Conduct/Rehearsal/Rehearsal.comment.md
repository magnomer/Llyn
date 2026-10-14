# Rehearsal

This folder holds the facts for the flashcard gates.

## `TDeck`

Covers the deck list gates over a real workspace engine.
Decks and folders a fact needs are made through `LDeckOutlet`, so only the gate under test asks the user.
The deck store holds no free grain, so every prepared deck is an entry deck.
The delete facts use the tally envoy, so a fact reads whether the cue count was shown.
The charter facts choose the deck through `CDeckSelect`, so the editor opens as the driver opens it.
  They read the saved deck back through `LDeckOutlet`, since the editor has no read gate.

## `TCue`

Covers the flashcard gates over a real workspace engine.
Decks and flashcards a fact needs are made through `LDeckOutlet`, as in `TDeck`.
The add fact saves its entries through a rig before the engine starts.
  No gate makes an entry without asking the user.
The facts read flashcards back through `LDeckOutlet`, since no gate lists them yet.
The cell facts use a free flashcard, the only kind whose cells the engine saves.

## `TStencil`

Covers the template gates over a real workspace engine.
The apply fact saves a template through `LDeckOutlet` and finds its id in the store.
  No engine read lists the saved templates yet.

## `TDepot`

Covers the deck export gate over a real workspace engine and a real phone folder.
Each fact saves the folder through `CLedgerDepotSave` and makes a free flashcard through `LDeckOutlet`.
The busy fact starts the workspace latch itself, so the engine refuses the run.
  The gate's busy read then answers true, since it reads the latch.
The pending fact exports once, then changes the written file as the phone app would.
The failing fact faults the export task through the fault atelier.
  The gate raises twice, once at the start and once at the end, both idle.
The refusal facts use the folder envoy, the fake that records a notice's refusal key.

## Stamp

Commit: `f6dae2392935`
- `TCue.cs` `66f51436b6aca5b8`
- `TDeck.cs` `931d9a9a5eff6096`
- `TDepot.cs` `a0a91aeea5fbe112`
- `TStencil.cs` `59e0b703b780f8b9`
