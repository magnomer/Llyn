# QGraspStar.cs
Hash: `dcd52f657675785d`

## `public sealed class QGraspStar`

Sizes, draws, hit-tests and steps the stars of one grasp row, so the veneer control only calls.
It reads and writes the row's step through the dependency properties the row hands it.
The limit is set on the row by its host from `LDisplay`, so this class names no Core constant.

## `public QGraspStar(FrameworkElement grasp, DependencyProperty step, DependencyProperty limit, RoutedEvent changed, RoutedEvent hovered, ImageSource? star)`

Keeps the row, its step and limit properties, its two events and the star image.
A missing star image throws here, so a broken asset fails before the row draws.
A disabled row is dimmed rather than hidden, so the header keeps its shape on a form standing on nothing.

## `public void QGraspStarRefine(IReadOnlyList<IReadOnlyList<int>> fill, IReadOnlyList<bool> unrated, IReadOnlyList<IReadOnlyList<int>> hit)`

Keeps the ready fills, looks and hit steps the host's driver hands over, then remeasures and redraws.
Until then the row holds no star, so it measures and draws none.
Drawing and pointing only look these answers up, so the control requests no logic.

## `public int? QGraspHover { get; private set; }`

The half step under the pointer, or null while the pointer is off the row.

## `public int QGraspPointed => QGraspHover ?? QGraspCurrent;`

The half step to word beside the stars.
It is the hovered one, else the set one.

## `public Size QGraspSizeDraw()`

One star frame for every hit pair the row holds, and the gaps between them.
Core counts the stars for the limit, so this sum adds only the view's own sizes.
A row with no star measures no star and no gap.
A slack border is added on every side.
Each star has a fixed size.
The slack lets a pointer near the row count as on it.

## `public void QGraspHoverRefine(Point point)`

Records which half star the pointer covers, redraws and raises the hover on every move.
It compares no step, since the step comes from the ready hit answers.

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
The shown step's fill list holds one entry per star, so it also sets how many stars draw.
Draws every star faded in the empty brush, or the unrated one where the answers mark the shown step.
An unrated row fades further, so it reads apart from a low rating.
The shown step is the hovered one, else the set one.
Then the fill brush is clipped to the whole or left half of each star, as its ready fill names.
The fills are looked up by the shown step, so this class keeps no fill arithmetic.
While the pointer hovers, the preview brush replaces the fill brush.

## `private void QGraspStarDraw(DrawingContext context, Brush ink, Rect frame)`

Paints one star frame in the brush, masked by the star image.

## `private void QGraspPreviewRefine(int? hovered)`

Records the hovered step, redraws the preview, and raises the hover event.

## `private void QGraspStepRefine(int step)`

Sets the step and raises the change.

## `private int QGraspStepDraw(Point point)`

Finds the star frame under a point and answers that half's step from the ready hit pairs.
A row with no star maps every point to zero.
Gaps and slack fold into the nearest star, so no point near the row is dead.
Which step a half picks, and the limit cap, are Core's answers.
