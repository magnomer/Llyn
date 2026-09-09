# PCardRegister.cs

## `internal sealed partial class PCard`

The Register field of one card: the chips it is marked with and the caret typed into.
Both sit in one collection so they wrap together as a single field.
It mirrors the Situation field, because a Register is shared data marked on a card the same way.
A duplicate wording is never added twice, so the field can never ask the engine to mark one card twice.

## `internal void PCardRegisterShow(IReadOnlyList<LRegisterDraft> drafts)`

Fills the field from a loaded card, dropping empty and repeated rows, and puts the caret back at the end.

## `internal IReadOnlyList<LRegisterDraft> PCardRegisterRead()`

The field as the engine takes it, including whatever stands unfinished in the caret.
Text left in the caret is not lost merely because the user never pressed Enter.

## `internal void PCardRegisterRemove(PRegister chip)`

Drops the chip the user clicked the cross on.

## `internal void PCardRegisterRemove(int step)`

Drops the chip on one side of the caret, for Backspace and Delete in an empty caret.

## `internal bool PCardRegisterMove(int step)`

Moves the caret one place along the field, for the arrow keys in an empty caret.

## `internal void PCardRegisterCommit()`

Turns whatever stands in the caret into a chip and empties the caret.

## `internal bool PCardRegisterCommit(string id, string name)`

Adds a chip for a Register already stored, chosen from the shelf.
It refuses a Register the card already marks and a wording already shown.

## `internal void PCardRegisterClear()`

Empties the caret without committing what stood in it.

## `internal bool PCardRegisterMatch(string id)`

Whether the card already marks the Register named by `id`, so the shelf never offers it twice.
