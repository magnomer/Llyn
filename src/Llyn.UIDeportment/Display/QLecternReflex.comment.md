# QLecternReflex.cs
Hash: `51a11764b210178b`

## `public sealed class QLecternReflex`

The reading view's reflex section, drawing the reflex rows [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md) answers.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID and hears its own hinge `PDisplayReflexHinge`.
The lectern subscribes its redraws to the display's open, close and notices.

## `public QLecternReflex(FrameworkElement surface, CDisplaySound area)`

Builds a `QReflexList` over the reflex list and the hinge, all pulled from `surface` with the loading line.
`area` is the display's sound area, the only part it reads.
The view's rows are only read, so nothing hears the list's typed cells.
The hinge's click comes straight here, so the sound strip holds no adapter for it.
Only the click is heard, so a repaint that sets the hinge writes nothing back.

## `public void QLecternReflexRefine()`

Draws the reflex block of the entry just opened, or empties it once the display closes.
The lectern subscribes it to both open and close, ahead of the fold.

## `public void QLecternRenewalRefine()`

Draws the reflex block again after a reflex fill, through the area's resonate, which reloads the rows.
The lectern subscribes it to the reflex notice, marshalled onto the page through `QObserver`, ahead of the fold.

## `public void QLecternFoldRefine()`

Has the list fold the rows by the shown entry's stored state, and set the hinge to match.
It runs after the rows are redrawn and on the display's fold bulletin for the shown entry.

## `public void QLecternAnchorRefine()`

Writes the anchors the area reads for the current rows onto the rows already drawn.
The lectern subscribes it to the fanqie notice, since a representative change moves no row.

## `private void QLecternHingeObserve(object sender, RoutedEventArgs e)`

Hears the hinge's click and hands its raw state to the gate `CDisplaySound.CDisplayReflexToggle`.
It hands the gate's verdict to `QLecternHingeRefine`.

## `private void QLecternHingeRefine(bool stored)`

Puts the hinge back to its previous state when the gate refused the write.
A stored write repaints through the fold bulletin, so it paints nothing then.

## `private void QLecternReflexRefine(CLecternReflex reflex)`

Has the list bring its rows up to the block in place and write their ready anchors.
It shows the loading line while a fill runs.
