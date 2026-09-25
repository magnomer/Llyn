# LGraspStar.cs

## `public sealed class LGraspStar`

Sizes, draws, hit-tests and steps the stars of one grasp row, so the veneer control only calls.
It reads and writes the row's step through the dependency properties the row hands it.
The limit is set on the row by its host from `LDisplay`, so this class names no Core constant.

## `public LGraspStar(FrameworkElement grasp, DependencyProperty step, DependencyProperty limit, RoutedEvent changed, RoutedEvent hovered, ImageSource? star, ImageSource? gray)`

Keeps the row, its step and limit properties, its two events and the two star images.
A missing star image throws here, so a broken asset fails before the row draws.
A disabled row is dimmed rather than hidden, so the header keeps its shape on a form standing on nothing.

## `public int? LGraspHover`

The half step under the pointer, or null while the pointer is off the row.

## `public int LGraspPointed`

The half step to word beside the stars: the hovered one, else the set one.

## `public static object LGraspStepClamp(DependencyObject sender, object value, DependencyProperty limit)`

Holds the step at or under the limit, since a step past the last star has nothing to draw.

## `public Size LGraspSizeResolve()`

Five stars and the four gaps between them, at a fixed size, plus a slack border on every side.
The slack lets a pointer near the row count as on it.

## `public void LGraspHoverHandle(Point point)`

Tracks which half star the pointer covers and redraws when it moves to another.

## `public void LGraspLeaveHandle()`

Drops the preview so the committed step shows again.

## `public void LGraspPressHandle(Point point)`

Focuses the row and sets the step the pointer covers.
Clicking the half that already equals the step clears to zero, so one click unrates.

## `public void LGraspKeyHandle(KeyEventArgs e)`

Left and Right move one half step, Home clears, End sets the limit.

## `public void LGraspDraw(DrawingContext context)`

A transparent rectangle over the whole row is drawn first, so the pointer hits the row, not only the ink.
Draws every star in gray, then the colored star clipped to the whole or left half for each earned step.
While the pointer hovers, the preview step is drawn fainter instead of the committed one.

## `private void LGraspHoverChange(int? hovered)`

Records the hovered step, redraws the preview, and raises the hover event.

## `private void LGraspStepChange(int step)`

Sets the step and raises the change, doing nothing when the step already stands.

## `private int LGraspStepResolve(Point point)`

Maps a point on the row to a half step from one to the limit.
The left half of a star is its odd step and the right half its even step.
Gaps and slack fold into the nearest star, so no point near the row is dead.
