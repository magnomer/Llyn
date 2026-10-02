# PContour.cs

## `public sealed class PContour : FrameworkElement`

The tone contour box drawn under an IPA reading of a tonal language.
It draws itself, one cell per syllable, on one guide line per level of the scale it is handed.
Each cell carries the syllable's pitch line and the syllable as written beneath it.
A level tone is a flat line and a contour tone bends at every level the marks spell.
Every level owns a theme colour.
A line between two levels fades from one colour to the other.
The box hides itself while the language is not tonal or the reading carries no tone.
The cells are sized so a contour is read at a glance, never as a glyph.

## `public static readonly DependencyProperty PContourSyllablesProperty`

The syllables the box draws, copied by the view that hosts the box from Conduct's ready contour.
Conduct answers none while the language is not tonal or the reading carries no tone.

## `public IReadOnlyList<QContourItem> PContourSyllables`

The syllables the box draws, copied by the driver from Conduct's contour records.
An empty list hides the box.

## `public static readonly DependencyProperty PContourScaleProperty`

The pitch levels the box draws a guide line for, highest first, set by the driver from Conduct's ready scale.

## `public IReadOnlyList<int> PContourScale`

The levels of the scale, highest first.
The first level is the top line and each next level lies one gap below.
An empty scale draws nothing.

## `public static readonly DependencyProperty PContourTopProperty`

The brush of the first level of the scale, the highest pitch, read from the theme.
`High`, `Mid` and `Low` follow it down the scale's order, and `Bottom` takes every lower level.

## `public static readonly DependencyProperty PContourGuideProperty`

The brush of the guide lines, read from the theme.

## `public static readonly DependencyProperty PContourAxisProperty`

The brush of the level digits along the left edge, read from the theme.

## `public static readonly DependencyProperty PContourInkProperty`

The brush of the syllable text under each cell, read from the theme.

## `public static readonly DependencyProperty PContourFrameProperty`

The brush the box is filled with, read from the theme.
It also rims every dot, so a dot stands off the line it sits on.

## `public static readonly DependencyProperty PContourEdgeProperty`

The brush the box is outlined with, read from the theme.

## `public static readonly DependencyProperty PContourFontProperty`

The phonetic family the syllable text is set in, read from the theme.

## `protected override Size MeasureOverride(Size availableSize)`

The box asks for its full width, one cell per syllable, and never shrinks to fit.
A shrunken contour would read as a glyph, which is what the box exists to avoid.

## `private double PContourPlotHeight`

The height the guide lines span, one gap per step of the scale.

## `private void PContourRefine()`

Shows the box while it holds syllables, and hides it otherwise.
It then asks for a new measure and a new drawing.

## `private Pen PContourLineBuild(IReadOnlyList<int> levels, IReadOnlyList<Point> points)`

A pitch line is stroked with a gradient laid along the cell in absolute coordinates.
Each turning point pins its level's colour at its own horizontal position.
So the colour between two points is the mix of their levels, and a level tone is one solid colour.

## `private IReadOnlyList<Point> PContourPointResolve(IReadOnlyList<int> levels, double left)`

The turning points spread evenly across the cell, inset from both edges.
A single level yields two points at the same height, so a level tone is drawn across the whole cell.

## `private double PContourLevelResolve(int level)`

The height of a level's line, one gap per step below the top level of the scale.

## `private int PContourDepthRead(int level)`

How many levels of the scale lie above this one, read from the scale's own order.
The box assumes no direction and no range of levels, so a scale in another order draws as handed.
The line height and the brush both follow this depth.
