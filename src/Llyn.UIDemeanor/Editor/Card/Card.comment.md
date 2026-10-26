# Card

Card holds the console's chip fields of one meaning or collocation card.
It is the console face of the GUI's chip fields, and it holds no entry rule.
Every verdict, list and chip comes from Conduct's caret, and these classes only draw it and route keys.

## `BPlacard`

This is the four chip fields of the lit card, taken together.
Their order is situation, register, tag and translation.
The order is the order of the card's parts, so a key can walk from one field to the next.
The part is the one field that may open.
The last part is the card's lead parts plus the four fields.
The lead parts are the header and the rows of `BCard`, which `BCardLeadRead` counts.
A folded card shows no chip field, so its last part is 0.
The part is clamped to that last part when read, so a fold never leaves it pointing at nothing.
Each field is built with a spot test that answers only while its part is the lit one.
So a field opens only on the card and the part the light is on.
The Escape row shuts every open field at once and keeps the draft.
It is live only while some field is open with no list showing, so a shown list takes Escape first.
Closing calls the keeper only when a field was open, so a quiet close writes nothing.
A change of the lit card resets every field, since text belongs to the card it was typed on.

## `BInlet`

This is one chip field, with the open entry the user types into.
It holds no chip, because the chips are read from the lit draft at each draw.
Each field differs only in the rules its owner hands the constructor.
Those rules are the chip reader, the caret's trail and three gates.
The first gate answers an edit, the second answers Enter, and the third answers a close.
Each gate returns the text the entry keeps and the rows of the list.
The caret trail decides where a new chip lands, so the field keeps no position.
The entry is drawn before the chip the trail names, or at the end when none is named.
A bare field shows its hint, and a field with a chip or typed text shows no hint.

### `BInletOpen`

A printable key opens the field before the field reads it, so the user types at once.
Opening again on the same card returns the text a close left, so abandoned words are not lost.

### `BInletClose`

A close settles what stands in the entry through the third gate.
Whatever the gate leaves is kept as the rest and shown as plain text beside the chips.

### `BInletRefine`

A different lit card resets the field.
A field that lost its spot while the card stays keeps its text as the rest.
So moving the light off a field never drops what was typed.

### `BInletKeymap`

Up and Down step the list through the caret's lantern, not through the list.
The lantern lives in Conduct, so both faces light the same row.
Enter picks the lit row when the lantern is on a row, and otherwise adds the typed text.
Escape shuts the list first and the field after.

### `BInletOfferRead`

It turns a Conduct offer into list rows and binds each row to its insert.
A hidden offer gives no rows, so a closed list is the same as an empty one.

### `BInletDraw`

The chips and the entry wrap as one run inside the card's width, divided by `BEditorSightDivide`.
The run is indented three cells, as the card's other rows are.
A token wider than the room is cut to the room, so a long word never breaks the line.
The line that holds the entry reports its caret column, and the others report none.

### `BInletDropDraw`

The list is drawn over the frame the drop builds.
The lit row is the caret's lantern, and it is shown lit only while the field has the focus.
Each row's mark is drawn in the hotkey tone after its lead.

## `BInletRow`

This is one row of a field's list in plain strings.
It carries the lead, the mark, the tail, the count and the choice to run.
The choice takes the card's kind and id, so a row inserts into the card it was offered for.

## Stamp

Commit: `c37ca1837ded`
- `BInlet.cs` `b78149e840d967c0`
- `BInletRow.cs` `ebe5c40765094e71`
- `BPlacard.cs` `c84041f8d271ae1d`
