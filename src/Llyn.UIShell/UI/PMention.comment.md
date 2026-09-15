# PMention.cs

## `public sealed class PMention : TextBlock`

The one control that draws a sentence a reader may click word by word.
It knows where every Mention lies and reports a click as a code-point offset.
It never talks to the engine.
The host that placed it asks the engine what the offset means and decides what to open.
So the display panel and the corpus panel draw a sentence the same way.
Each answers a click in its own way.

### `public static readonly DependencyProperty PMentionTextProperty`

The sentence text, the Mentions on it and the language it is written in.
A change to the text or the Mentions redraws the runs.
The language is carried for the host, which needs it when it asks the engine about an unlinked word.

### `public PMention()`

The face and size come from the card example keys, as the text block it replaced read them.
The display sets those keys on itself for the entry it shows, so nothing there changes.

### `protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)`

Where the button went down is kept, so the release can tell a click from a drag.

### `protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)`

A click is answered on release, and only when the pointer has not moved a drag's distance since the press.
A drag is the reader selecting text to copy, and it opens nothing.
In the reading view the band captures the mouse on a drag, so no release reaches here at all.
The event bubbles, so a host holding many sentences listens once above them all.

### `protected override void OnQueryCursor(QueryCursorEventArgs e)`

The cursor is a hand over a word linked to an Entry.
Anywhere else it is left to whoever answers next, which in the reading view is the band's beam.
An unlinked word may open an Entry too, but only the engine knows that, and the cursor does not ask.

### `private void PMentionShow()`

One run per piece, so the whole text is present and no gap is split into words.
The span of an unlinked word is the engine's to decide at click time, from the offset.
A linked run wears the linked style and a run standing for nothing wears the silent style.
A gap wears nothing.
Each run keeps its piece in its tag, which is how a click finds its offset.

### `internal Rect PMentionPieceRead(int offset)`

Where the character at a code-point offset is drawn, relative to the control.
The window asks so the menu can hang under the clicked word rather than under the sentence's left edge.
The offset is the one the engine settled on.
So an unlinked word inside a wide gap still places the menu on the word.
An offset no run covers answers the control's bottom left, which is where the popup would hang anyway.

### `private int? PMentionOffsetRead(Point point)`

The text pointer under the point is measured from the start of its own run, in UTF-16 units.
That count is turned into code points and added to the offset the run begins at.
A pointer outside every run, as in an empty control, answers nothing.
So does a lookup made while the runs are being rebuilt, which the text block refuses.
