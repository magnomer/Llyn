# Label

Label holds the Tag field of a card and the part-of-speech menu.
Tags and parts of speech are both labels the user may write anew.
So each field offers a menu but still accepts anything typed.

## `QCategoryItem`

This is one row of the `QCategory` menu, immutable because the menu is rebuilt rather than edited.
Its name is the one it hands the speech gate when clicked.
It also carries the engine's verdict that the entry already wears that name as a chip.
The row draws a check on that, so the menu and the chips read as one state.

## `QLabelChip`

This is one committed Tag as the boxed label the user sees.
The id is 0 when the chip is text the user typed.
The engine names it on the next draft save.
A Tag whose text should change is closed and written again rather than edited in place.

## `QLabelSelector`

The field holds committed Tags and one open entry in a single collection, so they wrap as one run.
That needs the item template chosen per item.

## `QCategory`

The menu offers the parts of speech the draft language's catalog lists, in its order.
Picking one commits its name as a chip.
The menu is an offer, not the field itself.
A pack cannot have thought of everything a user will want to write down.
So the field still takes anything typed, and what is typed anew joins the catalog.
The engine narrows the rows to the typed text and marks the taken ones.
Conduct hands the menu ready, so the driver only paints it.

### `QCategoryRefine`

When a hint key stands in place of the rows, the notice shows instead.
The key is Conduct's choice, so the notice only looks it up.
The marker's repaint and typing hand the menu in, so it follows every draft and keystroke.

### `QCategoryApply`

The check icon shows only when the row is taken.

### `QCategoryObserve`

The gate declares the name, so the chip carries the catalog's id.

### `QCategoryPickRefine`

It clears the field and closes the menu after a pick, as a committed Enter does.

## `QLabel`

This is the Tag field's driver, split by role.
Each gate names its card by the placard's collocation flag and id.
The engine finds a card by kind and id.
Each Observe hears one event and ends in one card gate, and each Refine only changes the look.
The first handler to handle a key ends its route.
Trim, blank and duplicate checks live in the Application clerk, not here.
An item's data context is the Tag or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.
This is shared with the Situation field, which is written the same way.
The slate driver is held too, since it finds its card through this one and cannot be built first.

### `QLabelShow`

The caret knows only chips, so this driver is the one place a Tag draft is read.
The anchor comes from Conduct's caret trail, so the entry stands where the trail placed it.

### `QLabelTextObserve`

Any other box's edit is ignored, since its data context is no Tag caret.
A box without the keyboard is ignored too, so a text the engine wrote back asks nothing.
`QSlateRefine` paints the answer, which is the text the entry keeps and the dropdown of stored Tags.

### `QLabelCommitObserve`

Enter runs after the dropdown's handlers, so Enter on a chosen stored tag never reaches it.
It never trims or checks the text, since the clerk owns that rule.
The answer is painted as an edit's is, so a refused settle leaves its text in the entry.

### `QLabelEraseObserve`

The key is taken at the edge even with no chip there.
The deletion is requested from the caret trail, which finds the chip and removes it.

### `QLabelCaretRefine`

The arrows ask the caret trail to step past a chip, and the screen only redraws the anchor it names.
A refused step reports no move, so the key falls through to the box.

### `QLabelBlurRefine`

It is subscribed before the close observer, so the dropdown shuts first.

### `QLabelCloseObserve`

Leaving the field commits what stands in the entry, so a Tag typed and abandoned is kept.
The entry then shows only what the engine answers as kept.

### `QLabelFocusRefine`

Clicking the field's empty space puts the caret in the entry.
So the whole box behaves as the one input it looks like.

### `QLabelFieldApply`

The selector is set only while none is, since a new selector would rebuild every item.

### `QLabelCardFind`

It answers null when no shown card holds the chip or caret.
`QSlate` asks it too, since a pick on a Tag caret lands on that card.

## Stamp

Commit: `f3a687d70b47`
- `QCategory.cs` `960340e0bfc6cf17`
- `QCategoryItem.cs` `97487f23cb2cc503`
- `QLabel.cs` `ebb5379dac5c778f`
- `QLabelChip.cs` `2d54f1d4412aa557`
- `QLabelSelector.cs` `a5c4bf2fefa4493a`
