# Field

The console's line of typed text.

## `BField`

A body for a sublevel box that takes one line of text.
It keeps the typed text, and reports every change to the callback it was given.
The callback lets the owner switch a button as the text changes.
It may start with a given text, which is not reported to the callback.

## `BFieldText`

The text typed so far, with the owner as its reader.
Only a key the field observes, or `BFieldClear`, changes it.

## `BFieldClear`

Empties the text without calling the edit callback.
The owner uses it when it has changed the text itself and has already refreshed what the callback would refresh.

## `BSParleyBodyObserve`

Backspace removes the last grapheme cluster, so a combining mark never stays alone.
A control character is not taken, so Enter and Escape reach the box.
Any other character is appended.

## `BSParleyBodyDraw`

Draws one `field` text of the given width, padded with blanks.
A text wider than the field shows its tail, kept to one cell short of the width.
The tail starts at a cluster boundary, so a wide character never splits.
A width of 0 or less draws nothing.
Cells are counted with the measure the box hands in, so the window's ambiguous-width choice applies.

## Stamp

Commit: `78fc0ebfff7c`
- `BField.cs` `16bdd439d402fe6d`
