# QGraspStar.cs
Hash: `a7b21edb79db715c`

## `public sealed class QGraspStar`

Sizes, draws, hit-tests and steps the stars of one grasp row, so the veneer control only calls.
It reads and writes the row's step through the dependency properties the row hands it.
The limit is set on the row by its host from `LDisplay`, so this class names no Core constant.

## `public QGraspStar(FrameworkElement grasp, DependencyProperty step, DependencyProperty limit, RoutedEvent changed, RoutedEvent hovered, ImageSource? star)`

Keeps the row, its step and limit properties, its two events and the star image.
A missing star image throws here, so a broken asset fails before the row draws.
A disabled row is dimmed rather than hidden, so the header keeps its shape on a form standing on nothing.

## `public int? QGraspHover`

The half step under the pointer, or null while the pointer is off the row.

## `public int QGraspPointed`

The half step to word beside the stars.
It is the hovered one, else the set one.

## `public static object QGraspLimitDraw(DependencyObject sender, object value, DependencyProperty limit)`

Holds the step at or under the limit, since a step past the last star has nothing to draw.

## `public Size QGraspSizeDraw()`

One star for every two steps of the limit and the gaps between them.
A slack border is added on every side.
Each star has a fixed size.
The slack lets a pointer near the row count as on it.

## `public void QGraspHoverRefine(Point point)`

Tracks which half star the pointer covers and redraws when it moves to another.

## `public void QGraspLeaveRefine()`

Drops the preview so the committed step shows again.

## `public void QGraspPressRefine(Point point)`

Focuses the row, sets the step the pointer covers, and raises the change even when it already stands.
Whether a press on the standing step clears it is the Conduct gate's rule, so the press always reports.

## `public void QGraspKeyRefine(KeyEventArgs e)`

Left and Right move one half step, Home clears, End sets the limit.
A key that lands on the standing step raises nothing, so the gate never reads it as a repeated press.

## `public void QGraspDraw(DrawingContext context, Brush fill, Brush empty, Brush unrated, Brush preview)`

A transparent rectangle over the whole row is drawn first, so the pointer hits the row, not only the ink.
The star image is only a mask, so the row's themed brushes give every star its colour.
Draws every star in the empty brush, or the unrated one while no step stands.
Then the fill brush is clipped to the whole or left half for each earned step.
While the pointer hovers, the preview brush replaces the fill brush.

## `private void QGraspStarDraw(DrawingContext context, Brush ink, Rect frame)`

Paints one star frame in the brush, masked by the star image.

## `private void QGraspPreviewRefine(int? hovered)`

Records the hovered step, redraws the preview, and raises the hover event.

## `private void QGraspStepRefine(int step)`

Sets the step and raises the change.

## `private int QGraspStepDraw(Point point)`

Maps a point on the row to a half step from one to the limit.
The left half of a star is its odd step and the right half its even step.
Gaps and slack fold into the nearest star, so no point near the row is dead.
