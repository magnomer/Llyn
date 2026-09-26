# LLecternAccent.cs

## `public sealed class LLecternAccent`

The reading view's accent deportment, standing between the veneer and [LDisplaySound](../../Llyn.Conduct/Display/LDisplaySound.comment.md).
It draws the primary pronunciation and the accent rows.
Flags come from `LEnsignImage`, and the respelling from `LRespellingMark`.

## `public void LLecternAccentAttach(LWindow window, UIElement surface, ColumnDefinition lead, Image flag, TextBlock label, TextBlock opener, TextBlock pronunciation, TextBlock closer, ItemsControl accents, DependencyObject contour, DependencyProperty tonal)`

Holds the window, the pronunciation surface and its parts, and binds the accent list to its rows.
`tonal` is the contour's own property, set as a value so no veneer type is named here.
The accent list is attached to `LAccentItem.LAccentItemApply`, which fills each row.

## `public void LLecternAccentShow()`

Draws the shown draft's language, flag verdict and primary pronunciation, handed over as parameters.

## `public void LLecternAccentClear()`

Collapses the pronunciation surface and its label column, and empties the accent rows.

## `private void LLecternAccentShow(string language, bool flagged, LPronunciationDraft? primary)`

Writes the primary pronunciation in its brackets and sets the contour's tone.
Rebuilds the accent rows, asking the pack once whether varieties draw as flags.
Flags not yet in the ensign store are loaded afterwards and painted in when they arrive.

## `private void LLecternSurfaceShow(string text)`

Writes the primary pronunciation, and collapses the surface and its label column when it is blank.

## `private void LLecternPrimaryShow()`

Draws the primary pronunciation's variety as a flag when one is known, and as a label otherwise.

## `private async Task LLecternFlagLoad(string language, bool flagged)`

Loads the variety flags the rows need and paints them once they are in the store.
Another entry shown meanwhile wins, so a late load paints nothing.
