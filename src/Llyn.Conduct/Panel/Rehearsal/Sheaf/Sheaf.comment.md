# Sheaf

This folder holds the cue gates: the listing of the chosen deck's cues and its adder.

## `CCue`

The flashcard gates of the chosen deck, built by `CDeck` beside the editor.
It reads the chosen deck from `CDeck` at each call and keeps no copy.
  So the chosen deck keeps one writer.
A gate that needs a deck does nothing while none is chosen.
`CCueChanged` tells the driver to read the deck's flashcards again after any change.

## `CCue` gates

`CCueRead` lists the chosen deck's flashcards as `CCueRow`, in the engine's order.
  It is empty while no deck is chosen, and a failure shows `Deck.LoadFailed`.
  It makes one engine call per listing and passes the portrait label down.
  `CCueRowLine` is the engine's line as given, so Conduct composes no text.

Each gate passes the engine's answer through and checks no flashcard rule.
The engine refuses a bound flashcard's cell edit with `Refusal.CueBound`.
  It also owns every column and index limit.
A read failure shows `Deck.LoadFailed`, and a change failure or a refusal shows `Deck.SaveFailed`.
`CCueMove` moves a flashcard to a position.
`CCueCreate` makes a free flashcard with no cells.
`CCueRemove` asks `Cue.RemoveConfirm` first, and a declined question removes nothing.
  An empty choice asks nothing.
`CCueSet` takes the flag as the driver's checkbox gives it, a bool.
  It turns the bool into the engine's int flag, one or zero, so no driver converts it.
`CCueCellRead` takes the selected cue ids as given, and answers null unless they name one free cue.
  So the driver neither counts the selection nor reads the grain, and a null hides the cell editor.
  It reads the chosen deck's cues for the grain, then the cells, so it makes two engine calls.
  It answers every slot `CStencil` offers, in that order, each with its stored value or an empty one.
  Each cell carries the slot's caption key and number, and whether it takes a file.
  A failed read shows `Deck.LoadFailed` and answers null.
`CCueCellSet` reads the cells, replaces the one at that column and index, and saves them all.
  That read is a second engine call, kept because the engine's save replaces every cell.
`CCueCellOpen` asks for a file through `CEnvoyMediaRead`, then saves its path as `CCueCellSet` does.
  The column chooses the title key and the suffixes, which are constants in `CCue`.
  Only a picture or a sound column opens a file, so another column throws.
  One private table, `LCueMediaRead`, answers both this choice and the read's file mark.
  A declined dialog keeps the cell.

## `CCueRow`

One flashcard of the listing as Conduct shows it.
Every field comes from the engine's cue, so Conduct derives none.
  The orphaned flag is the engine's own reading.
The position is the number `CCueMove` takes back.
The flag is a bool, the engine's number read as nonzero, so the driver's box takes it as it is.
The side texts are left out, because they record the last export.

## `CCueCell`

One cell of a flashcard as Conduct shows it.
The column is the engine's number.
The caption is a resource key and the number its suffix, both from the template's offer, as `CStencilSlot` carries them.
The file mark is true for a picture or a sound cell.
  So the driver shows the pick button without knowing columns.

## `CAccession`

The adder of the chosen deck, built by `CDeck` and read through `CDeckAccession`.
Its aperture holds the `accession` vista over the library entries, in headword order.
Each vista restore attaches a vista observer that raises `CApertureRowsChanged`.
  So a query set repaints the adder without the driver deciding when.
  `AccessionQuerySet_Headword_RaisesRowsChanged` pins it.
The vista bulletin runs on the thread that set the query, so the observer takes no marshal.
`CAccessionRead` lists the vista's entries as `CAccessionRow`, each with the engine's bound mark for the chosen deck.
  It is empty while no deck is chosen, and a failure shows `Accession.LoadFailed`.
  It keeps the rows as shown and sets the aperture's count from them.
`CAccessionAdd` adds one entry to the chosen deck, and `CAccessionShownAdd` adds every shown row not yet bound.
  It adds only rows the last read showed, and with no unbound shown row it makes no engine call.
  A failure shows `Deck.SaveFailed`, and a save raises `CAccessionChanged`.

## `CAccessionRow`

One library entry of the adder as Conduct shows it.
It carries the entry id, the headword, the epithet and whether the chosen deck already holds it.

## Stamp

Commit: `2f9a06b8eb95`
- `CAccession.cs` `48559274277838c2`
- `CAccessionRow.cs` `f608fa4fbe79fcbc`
- `CCue.cs` `080f47c84633d5ea`
- `CCueCell.cs` `0b6eb7f352958485`
- `CCueRow.cs` `532ef38db990c791`
