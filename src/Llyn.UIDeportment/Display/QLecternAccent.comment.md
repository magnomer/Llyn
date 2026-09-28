# QLecternAccent.cs

## `public sealed class QLecternAccent`

The reading view's accent driver, standing between the veneer and the display's area, [CDisplay](../../Llyn.Conduct/Display/CDisplay.comment.md).
It draws the primary pronunciation and the accent rows from the block Conduct answers ready.
Flags come from `LEnsignImage`, and every rule of which flag or reading shows stays in Conduct and below.

## `public QLecternAccent(CDisplay area)`

Builds the half over the display's area, whose reads it paints from.

## `public void QLecternAccentIntroduce(UIElement surface, ColumnDefinition lead, Image flag, TextBlock label, TextBlock opener, TextBlock pronunciation, TextBlock closer, ItemsControl accents, DependencyObject contour, DependencyProperty tonal)`

Holds the pronunciation surface and its parts, and binds the accent list to its rows.
`tonal` is the contour's own property, set as a value so no veneer type is named here.
The accent list is attached to `QAccentItem.QAccentItemRefine`, which fills each row.

## `public void QLecternAccentRefine()`

Answers an entry opening by drawing the block `CDisplayAccentRead` answers.

## `public async void QLecternEnsignRefine()`

Answers an entry opening by loading the flags the block draws, through the area's flag load.
The flags arrive after the rows are drawn, so each row then repaints its own flag.
A load answering nothing paints nothing, since another entry shown meanwhile wins.

## `public void QLecternMuteRefine()`

Answers an entry closing: collapses the surface and its label column, empties the rows and clears the flag.

## `private void QLecternAccentRefine(CLecternAccent accent)`

Writes the primary reading in the brackets Conduct chose and sets the contour's tone.
The surface and its label column collapse when the block has no primary reading.
Rebuilds the accent rows, each flagged by the block's verdict.

## `private void QLecternFlagRefine(CLecternAccent? accent)`

Repaints the drawn rows and the primary flag once the flags are in the store.

## `private void QLecternPrimaryRefine(CLecternAccent accent)`

Draws the primary pronunciation's variety as a flag when one is found, and as a label otherwise.
