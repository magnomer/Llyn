# Register

Register holds the Register field of a card, written the same way as the Tag and Situation fields.
A Register is a stored row the chip marks, so its chip carries the id the reference is written by.

## `PRegister`

This is one committed Register chip.
A chip made from typed text carries a fresh id, because the row it will become does not exist yet.
The wording is the engine's value, state and all, and the template's converter reads its mark.
Whether a pack names the Register is read from the stored row, so the chip carries no mark for it.

## `PRegisterSelector`

The field holds committed chips and one caret in a single collection.
The caret is the one row typed into, so it alone takes the entry template.

## `QRegister`

This is the Register field's driver, split by role.
Each Observe hears one event and ends in one card gate, and each Refine only changes the look.
The first handler to handle a key ends its route, and the dropdown's key handlers come first.
A chip or caret item's data context is the item, not the card.
So each handler finds the owning card by asking which card's caret is the item or holds it.

### `QRegisterShow`

The caret knows only chips, so this driver is the one place a Register draft is read.

### `QRegisterTextObserve`

Any other box's edit is ignored, since its data context is no Register caret.
`QProfferRegisterRefine` paints the kept text and the offered Registers.

### `QRegisterCommitObserve`

Enter runs after the dropdown's handlers, so Enter on a chosen stored Register never reaches it.
The wording goes to the engine as written, and the engine decides whether it names a stored Register.

### `QRegisterEraseObserve`

The key is taken at the edge even with no chip there.

### `QRegisterCaretRefine`

The arrows only walk the empty entry past a chip and ask no gate.

### `QRegisterBlurRefine`

It is subscribed before the close observer, so the dropdown shuts first.

### `QRegisterCloseObserve`

Leaving the field commits what stands in the entry, so a Register typed and abandoned is kept.

### `QRegisterFieldApply`

The selector is set only while none is, since a new selector would rebuild every item.

## Stamp

Commit: `c2f335c331fe`
- `PRegister.cs` `78f4581e696a5ffe`
- `PRegisterSelector.cs` `33a0ab2dc7c6bc68`
- `QRegister.cs` `886cc728a50a1213`
