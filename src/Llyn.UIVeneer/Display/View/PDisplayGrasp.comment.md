# PDisplayGrasp.cs

## `public partial class PDisplay`

The star row of the reading view: how well the user says they know the shown entry.
The row draws the stored step alone and a click writes the step straight to the workspace.
The words beside it name the step under the pointer, and the stored one when the pointer leaves.

## `private void PDisplayGraspShow(long id)`

Reads the shown entry's grasp and sets the star row to match.
A failed read leaves the row empty rather than claiming a rating.

## `private void PDisplayHoverHandle(object sender, RoutedEventArgs e)`

Re-words the label as the pointer moves across the stars, and back to the stored step when it leaves.

## `private void PDisplayGraspHandle(object sender, RoutedEventArgs e)`

Writes the step the click left on the star row onto the shown entry.
A refused write re-reads the stored step, so the row never shows a rating the workspace does not hold.
Rating creates no entry and changes no lexical data.
