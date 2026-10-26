# Card

Card holds the presentation models of one meaning or collocation card in the editor.
They are split from the drivers so a row can be redrawn from plain, comparable values.
The engine holds the data, and these models render it by id and report what the user does.

## `QPlacard`

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

### `QPlacardId`

The id is immutable and separates a card's address from its current list position.

### `QPlacardCollocated`

The list builds the placard with this flag, true for a collocation and false for a meaning.
It is immutable and rides with the id to every gate.
The engine finds a card by kind and id.
The image and video rows the placard builds take the flag and the id.
So a row's remove names its card.

### `QPlacardPosition`

A changed position notifies both the badge and the generated header title.
Badge text uses invariant culture and never comes from partially typed text.

### `QPlacardPositionActive`

Badge-edit visibility is presentation state, independent of the stored position.
Closing the badge changes it, which lets the row driver restore the position text.

### Wording echoes

Title, definition and expression remain Conduct's worded values.
Equal values are left alone, which avoids redundant redraws that could disturb the caret.

### `QPlacardFolded`

The fold verdict is read from the card of the placard's walk step, not kept here.
So the placard holds no fold policy of its own and the face cannot drift from Conduct.
A new step notifies the fold, and the row driver repaints the fold and the stored verdict together.

### `QPlacardStep`

The row of Conduct's walk that this placard shows, set at each refill.
It is the one source of the fold verdict, the stored verdict and the fold gate.
A placard not yet given a step reads as unfolded and not stored.

### `QPlacardStored`

The stored verdict is true while the step carries a fold gate, which Conduct gives only a stored card.
It lets the row driver decide hinge visibility.
It follows the step, so a card stored by a save gains its hinge at the next refill.

### `QPlacardPeek`

Folded text is read from the selected current wording.
So field updates cannot leave a separate peek copy stale.

## `QCardSentence`

This is the list of Example rows one card shows.
A row belongs to the card it was written under and moves with it.
The list knows no card, so its owner is named by whoever subscribes to its notice.
The particle, dependence and language lists are the editor's own, shared by every card.
One write reaches every card at once.

### `QCardSentenceNotice`

It carries a language picked for a Gloss, with its sentence row and the raw language.
The editor subscribes when it builds the card and hands the pick to one gate.
The list cannot send, because it holds no engine.
Typed text never comes this way, since a row holds no copy of what is typed.

### `QCardSentenceApply`

The card keeps no field order of its own.
A row opened later is built under the order the editor hands it.

### `QCardSentenceShow`

Rows are matched by sentence id, and a field already reading the engine's value is left alone.
The engine keeps a blank row, so a card always offers somewhere to write without the card adding one.

### Inline notes

### QCardSentence.cs: `private QPassage QCardSentenceCreate(CSentenceDraft draft, CSentenceOrder? order)`

The frame's order is applied at birth, so a row never draws in the default order first.
No order yet means no frame was read, and the row keeps its default order.

## `QInlet`

This is one chip field of a card together with the open entry the user types into.
The rows hold only the chips, and this object is itself the entry item the field's `QBerth` seats.
The entry is anchored to the chip it stands before, so showing the anchor never reorders the chips.
The field holds no position of its own, and Conduct's caret trail decides where a new chip lands.
Each field differs only in the rules its owner hands the constructor.
Those rules are pure functions and never reach back into the owner.
It knows only chips and never a Conduct draft, since the field's driver builds the chips.

### `QInletHint`

The hint belongs to the empty field, not to the entry.
Once a chip stands beside the entry, prompting again would read as a second, unfilled field.

### `QInletAnchor`

The anchor is null when the entry stands at the end.
Only `QInletAnchorShow` writes it, with the chip Conduct's trail names.
The field never moves the anchor itself, so the screen cannot drift from the trail.

### `QInletRowShow`

A standing chip with the same id stays unless the revise rule answers the fresh one.
The anchor is left as it is, so the driver shows it again after the row.

### `QInletClear`

The empty text reaches the gate as any edit does, and an empty text adds nothing.

### `QInletRefine`

The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a chip as a typed one does.

## Stamp

Commit: `c37ca1837ded`
- `QCardSentence.cs` `63b9a54f2bcd8e74`
- `QInlet.cs` `d28fd837c72eaa8f`
- `QPlacard.cs` `df764f58a38b96b2`
