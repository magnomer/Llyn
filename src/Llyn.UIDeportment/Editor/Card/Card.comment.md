# Card

Card holds the presentation models of one meaning or collocation card in the editor.
They are split from the drivers so a row can be redrawn from plain, comparable values.
The engine holds the data, and these models render it by id and report what the user does.

## `PCard`

One presentation item represents a meaning or a collocation without resolving Conduct's worded values.
It retains the card's identity, and changed values notify the row driver.
Unchanged values preserve the existing controls.
Shared sentence-menu lists keep cards consistent without copying their catalogs.
The caller's `peek` selects an existing wording, so no second copy of folded text exists.

### Chip fields

Image and video collections start empty and receive only supplied draft rows.
The panel never invents a blank row.
Rows are reconciled by id, so retained items accept fresh ready values.
Situation and register chips keep the standing item while identity and wording agree.
Translation chips keep the standing item only while headword and language both agree.
The bond list holds the chips of the meanings a collocation links to, in link order.
The first is its home.
The surface never holds a Conduct meaning.

### `PCardId`

The id is immutable and separates a card's address from its current list position.

### `PCardPosition`

A changed position notifies both the badge and the generated header title.
Badge text uses invariant culture and never comes from partially typed text.

### `PCardPositionActive`

Badge-edit visibility is presentation state, independent of the stored position.
Closing the badge changes it, which lets the row driver restore the position text.

### Wording echoes

Title, definition and expression remain Conduct's worded values.
Equal values are left alone, which avoids redundant redraws that could disturb the caret.

### `PCardFolded`

The last supplied fold verdict drives presentation, not a locally chosen fold policy.
Only a changed verdict notifies, and the row driver repaints the fold and the stored verdict together.

### `PCardStored`

The stored verdict comes from the initial draft and lets the row driver decide hinge visibility.
It is fixed for the card's lifetime, since rows are reconciled by the immutable id.

### `PCardPeek`

Folded text is read from the selected current wording.
So field updates cannot leave a separate peek copy stale.

## `PCardSentence`

This is the list of Example rows one card shows.
A row belongs to the card it was written under and moves with it.
The list knows no card, so its owner is named by whoever subscribes to its notice.
The catalog, particle, dependence and language lists are the editor's own, shared by every card.
One write reaches every card at once.

### `PCardSentenceNotice`

It carries a language picked for a Gloss, with its sentence row and the raw language.
The editor subscribes when it builds the card and hands the pick to one gate.
The list cannot send, because it holds no engine.
Typed text never comes this way, since a row holds no copy of what is typed.

### `PCardSentenceApply`

The card keeps no field order of its own.
A row opened later is built under the order the editor hands it.

### `PCardSentenceShow`

Rows are matched by sentence id, and a field already reading the engine's value is left alone.
The engine keeps a blank row, so a card always offers somewhere to write without the card adding one.

### Inline notes

### PCardSentence.cs: `private PSentence PCardSentenceCreate(CSentenceDraft draft, CSentenceOrder? order)`

The frame's order is applied at birth, so a row never draws in the default order first.
No order yet means no frame was read, and the row keeps its default order.

## `PCaret`

This is one chip field of a card together with the open entry the user types into.
The rows hold only the chips, and this object is itself the entry item the field's `QBerth` seats.
The entry is anchored to the chip it stands before, so showing the anchor never reorders the chips.
The field holds no position of its own, and Conduct's caret trail decides where a new chip lands.
Each field differs only in the rules its owner hands the constructor.
Those rules are pure functions and never reach back into the owner.
It knows only chips and never a Conduct draft, since the field's driver builds the chips.

### `PCaretHint`

The hint belongs to the empty field, not to the entry.
Once a chip stands beside the entry, prompting again would read as a second, unfilled field.

### `PCaretAnchor`

The anchor is null when the entry stands at the end.
Only `PCaretAnchorShow` writes it, with the chip Conduct's trail names.
The field never moves the anchor itself, so the screen cannot drift from the trail.

### `PCaretRowShow`

A standing chip with the same id stays unless the revise rule answers the fresh one.
The anchor is left as it is, so the driver shows it again after the row.

### `PCaretClear`

The empty text reaches the gate as any edit does, and an empty text adds nothing.

### `PCaretRefine`

The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a chip as a typed one does.

## Stamp

Commit: `c6e60141a172`
- `PCard.cs` `7357f03389c2c351`
- `PCardSentence.cs` `ae3ddd5aef584c1f`
- `PCaret.cs` `d289b16aa6ba7486`
