# PMention.cs
Hash: `fcb80ca8ba884202`

## `public sealed class PMention : TextBlock`

The one control that draws a sentence a reader may click word by word.
It knows where every Mention lies and reports a click as a code-point offset.
It never talks to the engine.
Its host hands it the sentence already divided into pieces by Conduct, so it never divides a text itself.
The host window reaches it as an inherited attached property the window sets once on itself.
Through it the block converts between code points and UTF-16 units.
A block not yet under the window reports no offset.
The host that placed it asks the engine what the offset means and decides what to open.
So the display panel and the corpus panel draw a sentence the same way.
Each answers a click in its own way.

### `public static readonly DependencyProperty PMentionHostProperty`

The host window, inherited down the tree so a block inside a template still finds it.
The block reads the host's atelier for its conversion gates, so no surface is handed a Conduct object.

### `public static readonly DependencyProperty PMentionSentenceProperty`

The id of the sentence row a reading-view line draws, a handle and not a copy of the sentence.
The reading view hands it back to its find gate, which reads the text and Mentions itself.
It draws nothing, so a change to it redraws nothing.

### `public PMention()`

The face and size come from the card example keys, as the text block it replaced read them.
The display sets those keys on itself for the entry it shows, so nothing there changes.

### `internal IReadOnlyList<QMentionPiece>? PMentionPiece`

The runs to draw, copied from the pieces Conduct divided the text into.
A change redraws the runs.
It is the control's only content input, so every host hands pieces and no host hands a bare text.
A placeholder word or an etymology prose arrives as one plain piece at code point zero.
So it still reports a click offset.
Null draws nothing.

### `protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)`

Where the button went down is kept, so the release can tell a click from a drag.

### `protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)`

A click is answered on release, and only when the pointer has not moved a drag's distance since the press.
The offset under the pointer is read before that test, since reading it decides nothing.
A drag is the reader selecting text to copy, and it opens nothing.
In the reading view the band captures the mouse on a drag, so no release reaches here at all.
The event bubbles, so a host holding many sentences listens once above them all.

### `private void PMentionShow()`

One run per piece, so the whole text is present and no gap is split into words.
Each run takes its text from the piece, so the control converts no span itself.
It calls no gate, since the host handed it the pieces ready.
The span of an unlinked word is the engine's to decide at click time, from the offset.
A linked run wears the linked style and a run standing for nothing wears the silent style.
A gap wears nothing.
Each run keeps its piece in its tag, which is how a click finds its offset.

### `internal Rect PMentionPieceRead(int offset)`

Where the character at a code-point offset is drawn, relative to the control.
The window asks so the menu can hang under the clicked word rather than under the sentence's left edge.
The offset is the one the engine settled on.
Each run asks the unit gate with its own start, and an offset outside the run answers null.
An empty character box is passed over like a run that misses.
So an unlinked word inside a wide gap still places the menu on the word.
An offset no run covers answers the control's bottom left, which is where the popup would hang anyway.

### `private int? PMentionOffsetRead(Point point)`

The text pointer under the point is measured from the start of its own run, in UTF-16 units.
That count is turned into code points and added to the offset the run begins at.
A pointer outside every run, as in an empty control, answers nothing.
So does a lookup made while the runs are being rebuilt, which the text block refuses.

## `private static void PMentionStyleApply(Run run, bool? linked)`

A linked run wears the linked style and a run standing for nothing wears the silent style.
Plain text between Mentions keeps the block's own style.

## `private static int PMentionOffsetRead(int start, int offset)`

The code-point offset of a click, from the piece's start and the offset inside it.
