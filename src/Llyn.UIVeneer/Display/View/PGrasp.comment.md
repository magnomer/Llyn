# PGrasp.cs

## `public sealed class PGrasp : FrameworkElement`

A row of stars the user clicks to say how well they know an entry, half a star per step.
It draws itself, so there is no child element per star and no template to keep in step.
The step it holds is a count of half stars from zero to the limit, in the store's own unit.
The limit comes from the deportment through `PGraspLimit`, so the row names no Core constant.
The row only maps a click to a step and raises it, and never decides what the store holds.

## `public static readonly DependencyProperty PGraspLimitProperty`

The last step, set once by the host from its deportment before any step is shown.
It measures the row, since the star count is half of it.

## `public static readonly DependencyProperty PGraspStepProperty`

The shown step, never negative and clamped to the limit.
So no surface can set a value the store would refuse.

## `public static readonly DependencyProperty PGraspFillProperty`

The brush a committed star is filled with, read from the theme.

## `public static readonly DependencyProperty PGraspEmptyProperty`

The brush every star's outline is drawn with, read from the theme.

## `public static readonly DependencyProperty PGraspUnratedProperty`

The brush every star's outline is drawn with while no step stands and nothing hovers, read from the theme.

## `public static readonly DependencyProperty PGraspPreviewProperty`

The brush the hover preview is filled with, read from the theme.

## `public static readonly RoutedEvent PGraspChangedEvent`

Raised after a click or key changed the step, carrying nothing but the sender.
The listener reads `PGraspStep` and writes it through the engine.

## `public static readonly RoutedEvent PGraspHoveredEvent`

Raised when the pointer moves onto another half star or leaves the row.
The listener reads `PGraspHover` and words the step beside the stars.

## `public PGrasp()`

A disabled row is dimmed rather than hidden, so the header keeps its shape on a form standing on nothing.

## `public int? PGraspHover`

The half step under the pointer, or null while nothing hovers.

## `public int PGraspPointed`

The half step to word beside the stars: the hovered one, else the set one.

## `protected override Size MeasureOverride(Size availableSize)`

Five stars and the four gaps between them, at a fixed size, plus a slack border on every side.
The slack lets a pointer near the row count as on it.

## `protected override void OnMouseMove(MouseEventArgs e)`

Tracks which half star the pointer covers and redraws when it moves to another.

## `protected override void OnMouseLeave(MouseEventArgs e)`

Drops the preview so the committed step shows again.

## `protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)`

Sets the step the pointer covers.
Clicking the half that already equals the step clears to zero, so one click unrates.

## `protected override void OnKeyDown(KeyEventArgs e)`

Left and Right move one half step, Home clears, End sets the limit.

## `private static void PGraspLimitHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

A new limit clamps the step already held.
So a limit set after the step never leaves it out of range.

## `private static object PGraspStepClamp(DependencyObject sender, object value)`

Holds the step at or under the limit, since a step past the last star has nothing to draw.

## `private void PGraspHoverChange(int? hovered)`

Records the hovered step, redraws the preview, and raises the hover event.

## `private void PGraspStepChange(int step)`

Sets the step and raises the change, doing nothing when the step already stands.

## `private static int PGraspStepResolve(Point point)`

Maps a point on the row to a half step from one to ten.
The left half of a star is its odd step and the right half its even step.
Gaps and slack fold into the nearest star, so no point near the row is dead.

## `private static Geometry PGraspStarBuild()`

The star symbol from `star.svg` scaled once into the sixteen-unit box every star is drawn in.
Half the stroke width is kept as inset, so the outline never leaves the box.
The loaded symbol arrives frozen, so a clone carries the transform.

## `private void PGraspDraw(DrawingContext context)`

A transparent rectangle over the whole row is drawn first, so the pointer hits the row, not only the ink.
Draws every star's outline, then a fill clipped to the whole or left half for each earned step.
An unrated row outlines in the muted brush, so the accent waits for a step or a hover.
While the pointer hovers, the preview step and brush are drawn instead of the committed ones.
