# Field

The console's line of typed text.

## `BField`

A body for a sublevel box that takes one line of text.
It keeps the typed text, and reports every change to the callback it was given.
The callback lets the owner switch a button as the text changes.

## `BFieldText`

The text typed so far, with the owner as its reader.
Only a key the field observes changes it.

## `BSParleyBodyObserve`

Backspace removes the last grapheme cluster, so a combining mark never stays alone.
A control character is not taken, so Enter and Escape reach the box.
Any other character is appended.

## `BSParleyBodyDraw`

Draws one `field` text of the given width, padded with blanks.
A text wider than the field shows its tail, kept to one cell short of the width.
The tail starts at a cluster boundary, so a wide character never splits.
A width of 0 or less draws nothing.

## Stamp

Commit: `9a85a3ab82db`
- `BField.cs` `ed3c30067f7dd421`
