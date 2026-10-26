# Field

The console's line of typed text.

## `BField`

A body for a sublevel box that takes one line of text.
It keeps the typed text, and reports every change to the callback it was given.
The callback lets the owner switch a button as the text changes.
It may start with a given text, which is not reported to the callback.
It may also take a hint, a catalog key shown while the text is empty.
So a hint needs the catalog.
The owner may give two edge callbacks, one for a removal and one for a step.
The field uses them when a key has nothing left to do inside it.

## `BFieldText`

The text typed so far, with the owner as its reader.
Only a key the field observes, or `BFieldClear`, changes it.
Typed characters wait in a pending buffer until they are applied.
So the text lags a typed run until the loop settles it.

## `BFieldClear`

Empties the text without calling the edit callback.
It also drops the pending characters and puts the caret back at the end.
The owner uses it when it has changed the text itself and has already refreshed what the callback would refresh.

## `BSParleyBodyObserve`

A plain character is not applied at once.
It joins the pending buffer and defers an apply onto the loop.
The first apply takes the whole buffer, so the apply deferred by each later character finds nothing to do.
So a run of typed characters makes one edit and one report to the callback.
While the apply waits, the field is listed on the loop, and it leaves the list when the apply runs.
`BFieldDeferredApply` applies every listed field at once, so a key row that reads the text first finds it whole.
Without the loop as the context, the character is applied at once.
Any other key applies the pending characters first and then itself, so keys keep the order they were pressed in.
Only a key that changes the text or the caret, and an edge key, is taken.
A control character that is none of those is not taken, so Enter and Escape reach the box.
A high surrogate waits for its low half.
So one emoji arrives as one character and never as two broken halves.
The editing rules live in `BCaret`.

## `BSParleyBodyDraw`

Draws one `field` text of the given width, padded with blanks.
An empty text with a hint draws the hint in the `muted` role over a blank field.
A text that leaves no room for the caret cell scrolls so the caret stays in view.
A `<` at the left edge and a `>` at the right edge say that text lies beyond.
A width under 3 has no room for the marks, so it only scrolls.
The window starts at a cluster boundary.
A wide character cut by an edge shows as blanks, so none splits.
A selection draws over the field in the `chosen` role.
A width of 0 or less draws nothing.
Cells are counted with the measure the box hands in, so the window's ambiguous-width choice applies.

The field puts the caret in the frame as a zero-width text of the `caret` role.
It does so only while it is focused.
Its row and column are read from the field's own caret and scroll.
The window and the canvas never recompute them.
So the terminal cursor lands where the field says the next character goes.

## `BCaret`

The key rules of a field, as pure functions of the text, the caret and the selection anchor.
The field keeps the state and applies the result, so the rules can be tested without a field.
Arrows, Home and End move by cluster or, with Control, by word.
Shift keeps the anchor to grow a selection.
A typed character or a removal replaces the selection first.
Backspace and Delete remove a whole cluster, so a combining mark never stays alone.
A step or removal that has no room left in the text goes to the owner's edge callback.
That holds only when nothing is selected.
So an empty field lets the arrows reach the owner, which can move a list or a choice beside it.

## Stamp

Commit: `c05429890cec`
- `BCaret.cs` `81459c2623ba4389`
- `BField.cs` `b3763d102be4ae00`
