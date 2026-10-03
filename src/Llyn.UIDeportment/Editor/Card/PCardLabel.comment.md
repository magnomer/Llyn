# PCardLabel.cs
Hash: `ead6a716b4c3f4e8`

## `internal sealed partial class PCard`

The Tags a card shows, held as the items of one field.
The collection holds only the chips, and the open entry beside them is the caret.
The caret is anchored to the chip it stands before, so moving it never reorders the chips.
The engine holds the chips, the card renders them by id, and every commit or removal is a request.
The engine's clerk skips a text the card already holds, and hands every text ready trimmed.

## `internal string PCardLabelText`

What is standing in the entry.

## `internal PLabelCaret PCardLabelCaret`

The caret the field's entry paints, which the editor finds a focused entry's card by.

## `internal int PCardLabelPosition`

How many chips stand before the caret's anchor, which is the place a new chip is asked for.
A caret with no anchor, or one whose chip is gone, stands at the end.

## `internal void PCardLabelShow(IReadOnlyList<CTagDraft> drafts)`

Makes the chips show the engine's Tags, matched by id, replacing a chip whose text changed.
The caret stays before the first chip that followed it and still stands.
A chip the engine adds at the caret therefore lands before it.

## `internal PLabelChip? PCardLabelFind(int step)`

The chip standing one step from the entry, before it or after it, or null.

## `internal bool PCardLabelMove(int step)`

Steps the entry one place along the field by anchoring it to another chip.
Reports whether there was anywhere left to go.

## `internal void PCardLabelClear()`

Empties the entry after a commit or a pick.
The empty text reaches the gate as any edit does, and an empty text adds nothing.

## `internal void PCardLabelRefine(string rest)`

Puts the text the tag gate keeps in the entry.
The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a tag as a typed one does.

## Inline notes

### `private void PCardLabelUpdate()`

The hint belongs to the empty field and goes once a chip stands beside the entry.
