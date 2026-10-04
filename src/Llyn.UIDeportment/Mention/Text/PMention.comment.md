# PMention.cs
Hash: `e684c80b87352e02`

## `public sealed class PMention : TextBlock`

The one control that draws a sentence a reader may click word by word.
It knows where every Mention lies and reports a click as surface values over its whole text.
It never talks to the engine.
Its host hands it the sentence already divided into pieces by Conduct, so it never divides a text itself.
The host window reaches it as an inherited attached property the window sets once on itself.
A block not yet under the window reports no click.
It places a popup from a ready UTF-16 unit in its whole text, and converts nothing.
The driver that placed it hands the click values to a gate, which decides what to open.
So the display panel and the corpus panel draw a sentence the same way.
Each answers a click in its own way.

## `public static readonly DependencyProperty PMentionHostProperty`

The host window, inherited down the tree so a block inside a template still finds it.
The block reads it only to tell whether it stands under the window yet.
Neither a click nor a popup place calls a gate, so no surface is handed a Conduct object.

## `public static readonly DependencyProperty PMentionSentenceProperty`

The id of the sentence row a reading-view line draws, a handle and not a copy of the sentence.
The reading view hands it back to its find gate, which reads the text and Mentions itself.
It draws nothing, so a change to it redraws nothing.

## `public PMention()`

The face and size come from the card example keys, as the text block it replaced read them.
The display sets those keys on itself for the entry it shows, so nothing there changes.

## `internal IReadOnlyList<QMentionPiece>? PMentionPiece`

The runs to draw, copied from the pieces Conduct divided the text into.
A change redraws the runs.
It is the control's only content input, so every host hands pieces and no host hands a bare text.
A placeholder word or an etymology prose arrives as one plain piece at code point zero.
So it still reports a click.
Null draws nothing.

## `protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)`

Where the button went down is kept, so the release can tell a click from a drag.

## `protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)`

A click is answered on release, and only when the pointer has not moved a drag's distance since the press.
The pointer must also land on a tagged run, and that hit-test is pure surface work.
The click raises its argument with the whole shown text and the unit in it, and calls no gate.
The driver that hears the event makes the one gate call.
A drag is the reader selecting text to copy, and it opens nothing.
In the reading view the band captures the mouse on a drag, so no release reaches here at all.
The event bubbles, so a host holding many sentences listens once above them all.

## `private void PMentionShow()`

One run per piece, so the whole text is present and no gap is split into words.
Each run takes its text from the piece, so the control converts no span itself.
It calls no gate, since the host handed it the pieces ready.
The span of an unlinked word is the engine's to decide at click time, from the offset.
A linked run wears the linked style and a run standing for nothing wears the silent style.
A gap wears nothing.
Each run keeps its piece in its tag, which marks the runs that make up the shown text.
The pieces cover the text from code point zero with no gap, so the runs in order are that text.

## `internal Rect PMentionPlaceRead(int? unit)`

Where the character at a UTF-16 `unit` of the whole shown text is drawn, relative to the control.
The window asks so the menu can hang under the found word rather than under the sentence's left edge.
The unit comes ready on the gate's offer, so the control calls no gate and names no Conduct type.
It walks the tagged runs in order, adding their lengths, to the run that holds the unit.
So a word that starts in an earlier run than the click is still found.
An empty character box answers like a unit no run holds.
A null unit, or one no run holds, answers the control's bottom left, where the popup would hang anyway.

## `private PMentionArgument? PMentionRunFind(Point point)`

The click argument for the tagged run under the point.
It carries the whole shown text, the tagged runs joined in order.
Its unit is the pointer's UTF-16 unit inside the run plus the lengths of the runs before it.
It calls no gate, so the release decides the gesture on surface state alone.
A pointer outside every run, as in an empty control, answers nothing.
So does a lookup made while the runs are being rebuilt, which the text block refuses.
A block not yet under the window answers nothing too.

## `private static void PMentionStyleApply(Run run, bool? linked)`

A linked run wears the linked style and a run standing for nothing wears the silent style.
Plain text between Mentions keeps the block's own style.
