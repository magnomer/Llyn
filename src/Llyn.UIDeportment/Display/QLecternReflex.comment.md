# QLecternReflex.cs
Hash: `a59219110c99173e`

## `public sealed class QLecternReflex`

The reading view's reflex section, drawing the reflex rows [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md) answers.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID and hears its own toggle and the sound area's fold change.
The lectern subscribes its redraws to the display's open, close and notices.

## `public QLecternReflex(FrameworkElement surface, CDisplaySound area)`

Builds a `QReflexList` over the reflex list and the fold toggle, all pulled from `surface` with the loading line.
`area` is the display's sound area, the only part it reads.
The view's rows are only read, so nothing hears the list's typed cells.
The fold toggle's events come straight here, so the sound strip holds no adapter for them.
The fold repaint is subscribed to the area's fold change, so every lectern follows one toggle.

## `public void QLecternReflexRefine()`

Draws the reflex block of the entry just opened, or empties it once the display closes.
The lectern subscribes it to both open and close, ahead of the fold.

## `public void QLecternRenewalRefine()`

Draws the reflex block again after a reflex fill, through the area's resonate, which reloads the rows.
The lectern subscribes it to the reflex notice, marshalled onto the page through `QObserver`, ahead of the fold.

## `public void QLecternFoldRefine()`

Has the list hide or show the folded rows from the shared fold, and set the toggle to match.
It runs after the rows are redrawn and whenever the fold gate changes the fold.
Writing the toggle back to the same value lets its event settle at once.

## `public void QLecternAnchorRefine()`

Writes the anchors the area reads for the current rows onto the rows already drawn.
The lectern subscribes it to the fanqie notice, since a representative change moves no row.

## `private void QLecternFoldObserve(object sender, RoutedEventArgs e)`

Hears the fold toggle and hands its state to the fold gate.

## `private void QLecternReflexRefine(CLecternReflex reflex)`

Has the list bring its rows up to the block in place and write their ready anchors.
It shows the loading line while a fill runs.
