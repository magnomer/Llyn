# PGrasp.cs

## `public sealed class PGrasp : FrameworkElement`

A row of five stars the user clicks to say how well they know an entry.
It draws itself, so there is no child element per star and no template to keep in step.
The step it holds is a count of half stars from zero to ten, the same unit the store keeps.
The row only maps a click to a step and raises it, and never decides what the store holds.

## `public static readonly DependencyProperty PGraspStepProperty`

The shown step, validated against `LGraspCheck` so no surface can set a value the store would refuse.

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

Left and Right move one half step, Home clears, End sets five stars.

## `public static string PGraspLabelResolve(int step)`

The localization key wording a step, one key per half step from zero to ten.
Both surfaces ask here, so a step is worded the same wherever it shows.

## `private void PGraspHoverChange(int? hovered)`

Records the hovered step, redraws the preview, and raises the hover event.

## `private void PGraspStepChange(int step)`

Sets the step and raises the change, doing nothing when the step already stands.

## `private static int PGraspStepResolve(Point point)`

Maps a point on the row to a half step from one to ten.
The left half of a star is its odd step and the right half its even step.
Every point maps to a step: gaps and slack fold into the nearest star, so nothing near the row is dead.

## `private static Geometry PGraspStarBuild()`

The Material star symbol scaled once into the sixteen-unit box every star is drawn in.
Half the stroke width is kept as inset, so the outline never leaves the box.
The parsed symbol arrives frozen, so a clone carries the transform.

## `private void PGraspDraw(DrawingContext context)`

A transparent rectangle over the whole row is drawn first, so the pointer hits the row and not only the ink.
Draws every star's outline, then a fill clipped to the whole or left half for each earned step.
An unrated row outlines in the muted brush, so the accent waits for a step or a hover.
While the pointer hovers, the preview step and brush are drawn instead of the committed ones.
