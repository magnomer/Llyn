# Register

Register holds the Register field of a card, written the same way as the Tag and Situation fields.
A Register is a stored row the chip marks, so its chip carries the id the reference is written by.

## `QRegisterChip`

This is one committed Register chip.
A chip made from typed text carries a fresh id, because the row it will become does not exist yet.
The wording is the engine's value, state and all, and the template's converter reads its mark.
Whether a pack names the Register is read from the stored row, so the chip carries no mark for it.

## `QRegisterSelector`

The field's chips and the one-item entry list `QBerth` builds beside them share this selector.
The caret is the one row typed into, so it alone takes the entry template.

## `QRegister`

This is the Register field's driver, split by role.
Each gate names its card by the placard's collocation flag and id.
The engine finds a card by kind and id.
Each Observe hears one event and ends in one Conduct gate, and each Refine only changes the look.
The first handler to handle a key ends its route, and the dropdown's key handlers come first.
A chip or caret item's data context is the item, not the card.
So each handler finds the owning card by asking which card's caret is the item or holds it.

### `QRegisterShow`

The caret knows only chips, so this driver is the one place a Register draft is read.
The anchor is the chip whose id Conduct's trail names, and no match leaves the entry at the end.

### `QRegisterTextObserve`

Any other box's edit is ignored, since its data context is no Register caret.
`QProfferRegisterRefine` paints the kept text and the offered Registers.

### `QRegisterCommitObserve`

Enter runs after the dropdown's handlers, so Enter on a chosen stored Register never reaches it.
The wording goes to the engine as written, and the engine decides whether it names a stored Register.

### `QRegisterEraseObserve`

The key is taken at the edge even with no chip there.
The deletion is requested from the caret trail, which finds the chip and removes it.

### `QRegisterCaretRefine`

The arrows ask the caret trail to step past a chip, and the screen only redraws the anchor it names.
A refused step reports no move, so the key falls through to the box.

### `QRegisterBlurRefine`

It is subscribed before the close observer, so the dropdown shuts first.

### `QRegisterCloseObserve`

Leaving the field commits what stands in the entry, so a Register typed and abandoned is kept.

### `QRegisterFieldApply`

The selector is set only while none is, since a new selector would rebuild every item.
The entry is built by `QBerth` beside the chips and stands where the card's caret is anchored.

## Stamp

Commit: `8c403d1df768`
- `QRegister.cs` `c4f5b453648729f2`
- `QRegisterChip.cs` `4bd6be4435d3fe84`
- `QRegisterSelector.cs` `6460ec5400d0447e`
