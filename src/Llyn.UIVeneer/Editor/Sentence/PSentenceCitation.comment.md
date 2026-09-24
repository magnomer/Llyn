# PSentenceCitation.cs

## `public partial class PEditor`

The citation field of an Example row: what a typed line becomes, and how rows show the Source they cite.
A citation is either a stored Source or nothing, so the field never keeps a line that matches no Source.

## `internal void PCitationKeyHandle(object sender, KeyEventArgs e)`

Drives the citation field from the keyboard.
While the dropdown stands open the arrows walk it and enter takes the selected row.
Enter with nothing selected commits the typed line, and escape puts the cited byline back.

## `internal void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field discards whatever was typed and not taken, and shuts the dropdown it opened.
A citation is either a stored Source or nothing, so a half-typed line is never kept.

## Inline notes

### `private static void PSentenceCitationReset(TextBox box)`

Puts the cited byline back in the field by reading its binding again.
The field keeps no line of its own, so discarding the typed one is only a re-read.

### `private void PSentenceCitationCommit(PCard card, PSentence row, TextBox box)`

Hands the typed line to the deportment, which resolves it against the held draft in one request.
The field then reads its binding again, so a failed line never stays.

### `private void PSentenceCitationSend(PCard card, PSentence row, long reference)`

Asks the engine to cite one picked Source on one row, or none when the id is zero.

### `private void PSentenceCitationShow()`

Has every row on the form read the byline of the Source it cites again.
It runs after the Source list is refilled, so a byline edited elsewhere is never stale here.
