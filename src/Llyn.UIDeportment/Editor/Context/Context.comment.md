# Context

Context holds the Situation field of a card.
It is separate from the other chip fields.
A Situation is stored under an id and written again, not edited.

## `QContextChip`

This is one committed Situation, the boxed wording the user sees.
It carries the id the Situation is stored under, so a card keeps naming the same Situation across saves.
The id is fixed at construction.
A Situation whose wording should change is closed and written again rather than edited in place.
A Situation the store could not read back is marked rather than shown.
The mark stands until the user closes the chip and writes the wording again.
So nothing unknown is quietly turned into nothing at all.

## `QContextSelector`

The field draws committed Situations and one open entry as a single run of items.
They wrap together as one run, which needs the item template chosen per item.

## `QContext`

This is the Situation field's driver, split by role.
Each Observe hears one event and ends in one card gate, and each Refine only changes the look.
The first handler to handle a key ends its route.
The dropdown's key handlers come first, so an open dropdown hears a key before the entry does.
An item's data context is the Situation or the entry, not the card.
So each handler finds the owning card by asking which card's caret is the item or holds it.

### `QContextShow`

The caret knows only chips, so this driver is the one place a Situation draft is read.
The anchor comes from Conduct's caret trail, so the entry stands where the trail placed it.

### `QContextTextObserve`

Any other box's edit is ignored, since its data context is no Situation caret.
The gate adds every Situation a comma completed and answers what the entry keeps.

### `QContextCommitObserve`

Enter runs after the dropdown's handlers, so Enter on a chosen stored Situation never reaches it.
The wording goes to the engine as written, and the engine decides which Situation it names.
A wording already stored is attached under its own id, so the workspace does not fill with twins.

### `QContextEraseObserve`

The key is taken at the edge even with no chip there.
The deletion is requested from the caret trail, which finds the chip and removes it.

### `QContextCaretRefine`

The arrows ask the caret trail to step past a chip, and the screen only redraws the anchor it names.
A refused step reports no move, so the key falls through to the box.

### `QContextBlurRefine`

It is subscribed before the close observer, so the dropdown shuts first.

### `QContextCloseObserve`

Leaving the field commits what stands in the entry, so a Situation typed and abandoned is kept.
Choosing a row empties the entry first, so the click that chose it commits nothing twice.

### `QContextFieldApply`

The selector is set only while none is, since a new selector would rebuild every item.

### `QContextFocusRefine`

Clicking the field's empty space puts the caret in the entry.
So the whole box behaves as the one input it looks like.

### `QContextCardFind`

It answers null when no shown card holds the chip or caret.
The dropdown asks it too, since a pick on a Situation caret lands on that card.

## Stamp

Commit: `2f9a06b8eb95`
- `QContext.cs` `ef1eb3f8f4dbe71b`
- `QContextChip.cs` `b695c0bf977b4a31`
- `QContextSelector.cs` `6ed10ad72c8e19f4`
