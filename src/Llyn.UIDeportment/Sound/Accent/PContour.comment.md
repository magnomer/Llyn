# PContour.cs
Hash: `2c22c21e13cf4b82`

## `public sealed class PContour : FrameworkElement`

The tone contour box drawn under an IPA reading of a tonal language.
It draws itself, one cell per syllable, on one guide line per level of the scale it is handed.
Each cell carries the syllable's pitch line and the syllable as written beneath it.
A level tone is a flat line and a contour tone bends at every level the marks spell.
Every level arrives with its theme brush, resolved by the Q driver from the role Core picked.
A line between two levels fades from one colour to the other.
The box hides itself while the language is not tonal or the reading carries no tone.
The cells are sized so a contour is read at a glance, never as a glyph.

## `public static readonly DependencyProperty PContourSyllablesProperty`

The syllables the box draws, copied by the view that hosts the box from Conduct's ready contour.
Conduct answers none while the language is not tonal or the reading carries no tone.

## `public PContour()`

The box starts collapsed, so it takes no room before any syllable arrives.
It never takes the pointer, so a press lands on what lies beneath it.
Each brush of its own and the font are theme references, so a theme change repaints the box.

## `public IReadOnlyList<QContourItem> PContourSyllables`

The syllables the box draws, copied by the driver from Conduct's contour records.
An empty list hides the box.

## `public static readonly DependencyProperty PContourScaleProperty`

The pitch levels the box draws a guide line for, highest first, set by the driver from Conduct's ready scale.

## `public IReadOnlyList<int> PContourScale`

The levels of the scale, highest first.
The first level is the top line and each next level lies one gap below.
An empty scale draws nothing.

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

## `public Brush PContourGuide`

The guide line brush, tied by the constructor to the theme's guide colour.

## `public Brush PContourAxis`

The level digit brush, tied by the constructor to the theme's axis colour.

## `public Brush PContourInk`

The syllable text brush, tied by the constructor to the theme's ink colour.

## `public Brush PContourFrame`

The fill and dot rim brush, tied by the constructor to the theme's frame colour.

## `public Brush PContourEdge`

The outline brush, tied by the constructor to the theme's edge colour.

## `public FontFamily PContourFont`

The face of the level digits and the syllable text, tied by the constructor to the theme's phonetic family.

## `protected override Size MeasureOverride(Size availableSize)`

The box asks for its full width, one cell per syllable, and never shrinks to fit.
A shrunken contour would read as a glyph, which is what the box exists to avoid.
The size is read from a `QContourPlot` built from the current scale and syllable count.

## `protected override void OnRender(DrawingContext drawingContext)`

Draws nothing until both the syllables and the scale are present.
One `QContourPlot` built from the scale and the syllable count places the guides and every cell.
The frame goes first and the guides next, so every pitch line sits on top of them.

## `private void PContourRefine()`

Shows the box while it holds syllables, and hides it otherwise.
It then asks for a new measure and a new drawing.

## `private Pen PContourLineBuild(IReadOnlyList<Brush> inks, IReadOnlyList<Point> points)`

A pitch line is stroked with a gradient laid along the cell in absolute coordinates.
Each turning point pins its brush's colour at its own horizontal position.
So the colour between two points is the mix of their levels, and a level tone is one solid colour.

## `private static Color PContourColorRead(Brush ink)`

The solid colour of a level's brush, for a gradient stop.
