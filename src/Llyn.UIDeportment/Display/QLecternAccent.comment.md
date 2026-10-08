# QLecternAccent.cs
Hash: `c85ebff88f1d4c47`

## `public sealed class QLecternAccent`

The reading view's accent section, standing between the veneer and the display's accent area, [CDisplayAccent](../../Llyn.Conduct/Display/CDisplayAccent.comment.md).
It draws the primary pronunciation and the accent rows from the block Conduct answers ready.
Flags come from `QEnsignImage`, and every rule of which flag or reading shows stays in Conduct and below.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID, and the lectern subscribes its redraws to the display's open and close.
It takes only the accent area, since that answers every read it paints from.

## `public QLecternAccent(FrameworkElement surface, CDisplayAccent area)`

Pulls the pronunciation surface, its parts, the contour box and the accent list from `surface`.
`area` answers every read it paints from.
The surface and its label column go to one [QLecternLead](QLecternLead.comment.md), which shows them together.
The contour's scale is set once here from Conduct's ready scale.
The contour is a framework element, so the theme brushes of its items resolve from it.
The accent list is bound to its rows and attached to `QAccentItem.QAccentItemRefine`, which fills each row.

## `public void QLecternAccentRefine()`

Draws the block `CDisplayAccentRead` answers.
The lectern subscribes it to both the display's open and close, since a closed display answers the mute block.
It writes the primary reading in the brackets Conduct chose and sets the contour's syllables.
`QContourInk.QContourInkBuild` turns the contour into items carrying each level's theme brush.
The surface and its label column collapse when the block holds nothing spoken.
Rebuilds the accent rows, each flagged by the block's verdict.

## `public async void QLecternEnsignRefine()`

Answers an entry opening by loading the flags the block draws, through the area's flag load.
The lectern subscribes it to the display's open only, after the block's redraw.
The flags arrive after the rows are drawn, so each row then repaints its own flag.
A load answering nothing paints nothing, since another entry shown meanwhile wins.

## `private void QLecternFlagRefine(CLecternAccent? accent)`

Repaints the drawn rows and the primary flag once the flags are in the store.

## `private void QLecternPrimaryRefine(CLecternAccent accent)`

Draws the primary pronunciation's variety as a flag when one is found, and as a label otherwise.
