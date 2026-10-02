# PCardLink.cs
Hash: `d507591e30abddac`

## `internal sealed partial class PCard`

The Translations a card shows, held as the items of one field the way the Tags are.
The collection holds only the chips, and the open entry beside them is the caret.
The caret is anchored to the chip it stands before, so moving it never reorders the chips.

A link holds only the id of the Entry it points at.
The headword, the language and the flag beside it are drawn from that Entry, not stored.
So a chip is a pointer the field happens to be able to read aloud.

Typed text is not a link until an Entry answers it.
The card cannot ask, because the search belongs to the engine and the card holds no engine.
So the entry's text is handed to the translation gate, which resolves each word and links it.
The engine holds the links, and the card renders them by id from the targets the editor read.

## `internal PLinkCaret PCardLinkCaret`

The caret the field's entry paints, which the editor finds a focused entry's card by.

## `internal int PCardLinkPosition`

How many chips stand before the caret's anchor, which is the place a new link is asked for.
A caret with no anchor, or one whose chip is gone, stands at the end.

## `internal void PCardLinkShow(IReadOnlyList<CTranslationTarget> targets)`

Makes the chips show the engine's links, matched by id.
Each target carries the headword and language its id stands for, read once for the whole card.
A chip whose ready target no longer equals the new one is replaced, since its target is fixed.
The caret stays before the first chip that followed it and still stands.
A chip the engine adds at the caret therefore lands before it.

## `internal QLinkChip? PCardLinkFind(int step)`

The chip standing one step from the entry, before it or after it, or null.

## `internal bool PCardLinkMove(int step)`

Steps the entry one place along the field by anchoring it to another chip.
Reports whether there was anywhere left to go.

## `internal void PCardLinkClear()`

Empties the entry once the word standing in it became a link.
The empty text reaches the gate as any edit does, and an empty text adds nothing.

## `internal void PCardFlagUpdate()`

Asks each chip to read its flag again in place.
Flags are loaded after the window opens, so an early chip can be drawn without one.
A chip keeps its slot, since only its image reloads.

## `internal void PCardLinkRefine(string rest)`

Puts the text the translation gate keeps in the entry.
The editor paints the dropdown the same answer carries.
The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a word as a typed one does.

## Inline notes

### `private void PCardLinkUpdate()`

The hint is read from the localization resources, so it speaks the language the window does.
The hint belongs to the empty field, not to the entry.
Once a card carries a link the entry sits beside it.
