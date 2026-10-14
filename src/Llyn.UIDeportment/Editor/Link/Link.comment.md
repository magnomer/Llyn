# Link

Link holds the Translation field of a card and the reflex anchor dropdown.
Both are popups or fields that are retargeted at whatever asked for them.

## `PLinkSelector`

The field's chips and the one-item entry list `QBerth` builds beside them share this selector.
So the template is chosen per item, the entry template for the caret and the chip template otherwise.

## `QAnchor`

The anchor dropdown is one popup the panel owns, retargeted at the reflex row that asked for it.
It lists the character's stored placements with a tick each, and a tick hands that pair to one gate.
A placement whose tone class the row's own tone may descend from ends with the estimate mark.
The mark stands at the end.
So the label reads as stored and the estimate reads as a comment on it.
Conduct holds the row the open popup edits, so the driver keeps no row between open and tick.
It takes the plain scope, since it calls no member of the editor.

### `QAnchorShutRefine`

It shuts an open popup before the anchor command reaches `QAnchorReflexObserve`, so it reopens under the new label.
Shutting it after the open gate could let the popup's close clear the row that gate just chose.

### `QAnchorTickObserve`

The row comes back from the draft change with its new label.

## `QLink`

This is the Translation field's driver, split by role.
Each Observe hears one event and ends in one Conduct gate, and each Refine only changes the look.
The first handler to handle a key ends its route.
Trim, blank, resolve and search rules live below Conduct, not here.
The caret gate holds the chip trail, so no handler here counts chips or reads a position.
An item's data context is the link or the entry, not the card.
So each handler asks which card's collection holds the item, as the Tag field does.
Items are built per card, so one routed text handler on the scope hears every Translation caret.
Every card the editor builds is heard, whether it was loaded or added by hand.

### `QLinkShow`

Each chip copies the target's id, headword and language, so no Conduct record rides the caret.
The anchor is the chip whose id Conduct's trail names, and no match leaves the entry at the end.

### `QLinkTextObserve`

Any other box's edit is ignored, since its data context is no Translation caret.
The gate links each completed word that resolves and answers the words that stay, with their dropdown.

### `QLinkRefine`

The rows, the word and the chosen row come ready, so nothing is decided here.

### `QLinkCommitObserve`

Enter runs after the dropdown's handlers, so Enter on a chosen row never reaches it.
The gate answers the dropdown, with one whole-headword match chosen already.

### `QLinkEraseObserve`

The key is taken at the edge even with no chip there.
The deletion is requested from the caret trail, which finds the chip and removes it.

### `QLinkCaretRefine`

The arrows ask the caret trail to step past a chip, and the screen only redraws the anchor it names.
A refused step reports no move, so the key falls through to the box.

### `QLinkBlurRefine`

It is subscribed before the close observer, so the dropdown shuts first.

### `QLinkCloseObserve`

A word one Entry answers becomes a link, and the entry empties.
Anything else stays in the entry rather than opening a dropdown the user has already left.

### `QLinkFlagRefine`

It redraws every card's chips once the workspace's flags have loaded.
It walks each card's link chips itself, since the card holds no flag rule.

### `QLinkFieldApply`

The selector is set only while none is, since a new selector would rebuild every item.
The entry is built by `QBerth` beside the chips and stands where the card's caret is anchored.

### Inline notes

### QLink.cs: `internal TextBox? QLinkBoxFind(PCard card)`

The dropdown hangs off the caret it was opened from.
The caret has no name to bind to, so the focused box is asked whether it is that card's.

### QLink.cs: `internal PCard? QLinkCardFind(object row)`

The Translation dropdown asks it too, since a pick on a Translation caret links on that card.

## Stamp

Commit: `c6e60141a172`
- `PLinkSelector.cs` `d070f64083422ea2`
- `QAnchor.cs` `942ece4b06597d3d`
- `QLink.cs` `aa3d59d46da4d415`
