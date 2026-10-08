# QLecternGrasp.cs
Hash: `0a6c93bcd741a9d4`

## `public sealed class QLecternGrasp`

The reading view's star row, drawing what [CDisplayGrasp](../../Llyn.Conduct/Display/CDisplayGrasp.comment.md) answers.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID, and the lectern subscribes its redraw to the display's open and close.

## `public QLecternGrasp(FrameworkElement surface, CDisplayGrasp area)`

Pulls the star row and its label from `surface`, and sets the row's limit once from the engine's last step.
`area` is the display's grasp area, the only part it reads.
The row's step and hover events come straight here, so the view holds no adapter for them.
The row redraws on the grasp notice marshalled through `QObserver`.

## `public void QLecternGraspRefine()`

Draws the stars and their label from the grasp the area answers for the shown entry.
The lectern subscribes it to both the display's open and close.

## `private void QLecternGraspObserve(object sender, RoutedEventArgs e)`

Hands the star row's new step to the gate, then draws the stars it answers.
A press on the standing step thus comes back cleared, since the gate owns that rule.

## `private void QLecternHoverRefine(object sender, RoutedEventArgs e)`

Words the step under the pointer beside the stars, or the set step once the pointer leaves.

## `private void QLecternGraspRefine(CGrasp grasp)`

Writes the ready grasp's step onto the stars and its label beside them.
