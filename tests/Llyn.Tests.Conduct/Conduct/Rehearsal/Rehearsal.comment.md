# Rehearsal

This folder holds the facts for the flashcard gates.

## `TDeck`

Covers the deck list gates over a real workspace engine.
Decks and folders a fact needs are made through `LDeckOutlet`, so only the gate under test asks the user.
Every deck a fact prepares is an entry deck.
The delete facts use the tally envoy, so a fact reads whether the cue count was shown.
The order facts read the roll after `CDeckOrderMove`, so the engine's stored order is what they check.
The grain facts pin the five creatable grains and their wording keys in order.
  A grain select in range is kept and one out of range is ignored.
  A created deck takes the kept grain.
The destination facts list the top level first for a foldered deck, and folders only for a top-level deck.
  The move fact takes the one destination a deck is offered.
The view facts pin that `CDeckViewSet` keeps a defined view once, ignores an undefined one, and survives `CDeckSelect`.

## `TCharter`

Covers the deck editor gates over a real workspace engine.
The facts choose the deck through `CDeckSelect`, so the editor opens as the driver opens it.
  The save facts read the saved deck back through `LDeckOutlet`, because `CCharterRead` reads only the draft.
Each gate has one save fact.
  The target fact saves letters and keeps the default target.
The draft fact reads `CCharterRead` after a gate, and the stored deck keeps its old lead.
  It also pins the caption keys of the two directions.
With no deck chosen, `CCharterRead` reads null.
A new deck's back of direction two holds side one, so the back fact flips side two.

## `TCue`

Covers the flashcard gates over a real workspace engine.
Decks and flashcards a fact needs are made through `LDeckOutlet`, as in `TDeck`.
The change facts read flashcards back through `LDeckOutlet`, apart from the gate under test.
The remove fact declines `Cue.RemoveConfirm` and keeps both flashcards.
The listing fact reads `CCueRead` on two free flashcards, in the engine's order.
  Their lines read empty, since they hold no cells.
With no deck chosen the listing is empty and asks nothing.
A listing whose engine read throws shows `Deck.LoadFailed` and reads no rows.
  The fault atelier throws on `LEngineLineRead` alone, so the deck is still chosen.
The cell facts use a free flashcard, the only kind whose cells the engine saves.
  The cell read fact on a fresh flashcard reads five empty text cells and marks picture and sound as files.
  A selection of two flashcards reads null.
The open fact declines the picture picker, asking `Media:Stencil.Picture`, and keeps the old picture cell.
The bound fact saves its entry through a rig before the engine starts.
  It binds a flashcard to that entry, then sets a cell on it.
  The set shows `Deck.SaveFailed`, and the cell read answers null.
The set facts pin the bool flag's conversion, a checked flag stored as one and a cleared flag as zero.
The flag read fact sets the flag through `CCueSet` and reads it back from `CCueRead` as true.

## `TStencil`

Covers the template gates over a real workspace engine.
The body fact sets a notes slot body and raises one change.
The apply fact saves a template through `LDeckOutlet` and finds its id in the store.
The saved-list fact saves a template the same way and reads it in `CStencilStateSaved`.
The offer fact reads a new entry deck.
  It holds 21 slots, the first holding the headword token, and an empty fourth category.
With no deck chosen, `CStencilRead` reads null.
The side facts read a literal body as a custom side with no chips, and field tokens as chips.
The field facts append a chip to side two, drop the first chip, and set a join to all.
The media facts set the sound side and the picture side through `CStencilMediaSet`, `CStencilPictureSet` and `CStencilSoundSet`.
The grain fact reads the meaning deck's grain.
The text fact toggles `CStencilTextChecked` twice, reads it checked then unchecked, and counts two changes.
The failure fact faults `LEngineFieldAppend`, so the append shows `Stencil.SaveFailed` and raises no change.

## `TAccession`

Covers the adder gates over a real workspace engine and library entries saved through the entry clerk.
The read fact adds one entry, then reads both entries in headword order with only the added one bound.
The add fact adds one entry as a cue.
The shown-add facts add every unbound row of the last read, and add nothing before any read.
The query fact sets a headword query on the aperture.
  The aperture raises `CApertureRowsChanged` once, and the next read holds only the matching entry.

## `TDepot`

Covers the deck export gate over a real workspace engine and a real phone folder.
The busy and pending facts save the folder through `CLedgerDepotSave`.
  They make a free flashcard through `LDeckOutlet`.
The busy fact starts the workspace latch itself, so the engine refuses the run.
  The gate's busy read then answers true, since it reads the latch.
The pending fact exports once, then changes the written file as the phone app would.
The failing fact faults the export task through the fault atelier.
  The gate shows `Deck.ExportFailed` once.
  It raises twice, both idle, with `Deck.Exporting` at the start and an empty line at the end.
The refusal facts use the folder envoy, the fake that records a notice's refusal key.

## `TStudy` and `TScoreboard`

Each pins the empty-line key its placeholder names.

## Stamp

Commit: `2f9a06b8eb95`
- `TAccession.cs` `1a0d66a8e98303ac`
- `TCharter.cs` `2adc5e66a188e32a`
- `TCue.cs` `91b47b58da593b72`
- `TDeck.cs` `e408a28c6a60f943`
- `TDepot.cs` `2d0b88b32165a1cc`
- `TScoreboard.cs` `bd5adb9c0d1f9bc0`
- `TStencil.cs` `c6a716c83630b270`
- `TStudy.cs` `a85efa937a1dc4a5`
