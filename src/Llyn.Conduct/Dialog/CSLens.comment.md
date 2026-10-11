# CSLens.cs
Hash: `f7d46996ac6ededb`

## `public sealed class CSLens`

The print preview's zoom and its rules, owned in Conduct.
Typed or wheeled zoom input is raw, so the driver hands it here and paints the answer.
It holds the factor and the fit mode as one immutable grade, replaced only through `LSLensFactorSet`.
It holds the last view size, and the proof's paper size with its default resolution, as two more immutable records.
Nothing about zoom goes below Conduct, so its gates call no engine member.
`CSGalley` builds one per galley and feeds it each proof.

## `private static readonly int[] CSLensLadder`

The zoom percents the step gate walks, ascending.
Its first and last rungs are also the clamp every factor obeys.

## `private CSLensGrade _csLensGrade`

The current zoom, width fit at a factor of 1 when the window opens.
Only `LSLensFactorSet` replaces it, whole.

## `private CSLensArea _csLensArea`

The size of the view the page is laid in, zero before the first layout.
`CSLensAreaSet` replaces it whole.

## `private CSLensPaper _csLensPaper`

The proof's paper and default resolution, zero when no proof is held.
`LSLensPaperSet` replaces it whole.

## `internal CSLens()`

Built only by `CSGalley`, so the zoom lives exactly as long as its galley.

## `public CSLensState CSLensRead()`

The current zoom as a ready answer.
The text is the factor as a whole percent in the current culture.

## `public CSLensState CSLensTextSet(string text)`

Takes the zoom box text as typed.
It drops the culture's percent sign and reads the rest as a whole number in the current culture.
A number sets a free factor of that percent, clamped to the ladder.
Text that does not read refits in the current mode, so the box shows the zoom again.

## `public CSLensState CSLensLadderMove(int delta)`

Steps the factor one rung along the ladder and frees it from fitting.
A positive delta steps up and any other steps down.
So the driver hands a raw wheel delta unread.
Past either end the factor stays on the end rung.
A factor between rungs steps to the nearest rung in that direction.

## `public CSLensState CSLensModeSet(CSLensMode mode)`

Chooses a fit mode and fits the factor to it.
Without a paper or a laid-out view the factor stays as it was.

## `public CSLensState CSLensAreaSet(double width, double height)`

Takes the raw size of the view the page is laid in and refits in the current mode.
A free factor stays as it was.

## `internal CSLensState LSLensPaperSet(double width, double height, int resolution)`

Called by `CSGalleyDraw` with each proof's paper size and default resolution, zeros after a failure.
It refits in the current mode, so a new paper or orientation keeps the chosen fit.

## `internal int? LSLensResolutionRead(double scale)`

The dots per inch a page needs at the current zoom on a monitor of `scale`, or null.
The shown width is capped at twice the view width, matching what the driver decodes.
It answers null while the proof's default resolution already covers that width.
No pixel width leaves this method, only a document resolution.

## `private double LSLensFitRead(CSLensMode mode)`

The factor `mode` fits, from the view size and the paper size.
A free mode, a missing paper or an empty view answers the current factor.

## `private CSLensState LSLensFactorSet(CSLensMode mode, double factor)`

The one writer of the zoom.
It clamps the factor to the ladder ends and answers the new zoom.

## `private sealed record CSLensGrade(CSLensMode CSLensGradeMode, double CSLensGradeFactor);`

The fit mode and the zoom factor, which always change together.
The factor is already clamped to the ladder ends.

## `private sealed record CSLensArea(double CSLensAreaWidth, double CSLensAreaHeight);`

The width and height of the view the page is laid in, in device-independent pixels.
It is the raw measure from the last `CSLensAreaSet`.

## `private sealed record CSLensPaper(double CSLensPaperWidth, double CSLensPaperHeight, int CSLensPaperResolution);`

The proof's first page width and height in device-independent pixels.
The resolution is the dots per inch the proof draws a page at when asked for no resolution.
