# QContourPlot.cs
Hash: `5ad3f823465cdf60`

## `internal sealed class QContourPlot`

The geometry of the tone contour box, kept apart from the inks and text the box paints with.
It places the guide lines, the cells, the pitch points and the labels from two values.
Those are the scale handed to `PContour` and the number of syllables it draws.
The box builds one for each measure and each drawing, so a plot never outlives the values it read.

## `internal const double QContourCellWidth`

The width of one syllable's cell.
The box caps the syllable text at this width, so a long syllable is trimmed inside its own cell.

## `internal QContourPlot(IReadOnlyList<int> scale, int count)`

Holds the scale, highest level first, and the number of syllables the box draws.

## `internal Size QContourPlotSize`

The box's full size, one cell per syllable, with room for at least one cell.
The height is the guide span plus the label row, inside the padding.

## `internal double QContourLabelTop`

Where the syllable text starts, one label gap below the lowest guide line.

## `private double QContourPlotHeight`

The height the guide lines span, one gap per step of the scale.

## `internal double QContourLeftRead(int index)`

The left edge of the cell at this index, past the padding and the level axis.
The guide lines start at the edge of the first cell.

## `internal double QContourRightRead(double width)`

Where the guide lines end, one padding inside the right edge of a box this wide.

## `internal double QContourLevelResolve(int level)`

The height of a level's line, one gap per step below the top level of the scale.

## `internal IReadOnlyList<Point> QContourPointResolve(IReadOnlyList<int> levels, int index)`

The turning points spread evenly across the cell at this index, inset from both edges.
A single level yields two points at the same height.
So a level tone is a flat line across the same span as a contour.

## `private int QContourDepthRead(int level)`

How many levels of the scale lie above this one, read from the scale's own order.
The plot assumes no direction and no range of levels, so a scale in another order draws as handed.
The line height follows this depth.
