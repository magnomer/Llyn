# QFootprint.cs
Hash: `3c5265be4b52dbbb`

## `public sealed class QFootprint`

Where the window was and how big it was, carried from one run to the next.
The window's posture holds it, so a workspace opens the way its owner left it.
It names no window, so the window view reads the screen and applies what it answers.

## `private const double QFootprintShare = 0.8;`

The share of the work area a first launch fills.
A designed pixel size cannot fit every desktop, so the default is measured against the screen.

## `private const int QFootprintDelay = 700;`

The rest in milliseconds the posture waits after a move before it writes.
A drag raises a change for every pixel, and one write per pixel would be a file write per pixel.

## `public QFootprint(QPosture posture)`

Keeps the posture.

## `public LCapsuleWindow? QFootprintRead(double left, double top, double width, double height, double minWidth, double minHeight)`

The stored geometry, placed inside the current virtual screen, or null when nothing is stored.
The screen's rectangle and the window's minimum size are handed in by the window view.
Stored geometry is clamped into that screen and against the minimum size.
A monitor that is gone, or a saved size larger than the desktop, must not leave the window unreachable.

## `public (double QFootprintWidth, double QFootprintHeight) QFootprintSizeRead(double width, double height, double minWidth, double minHeight)`

The size a first launch takes is its share of the work area, never below the minimum size.

## `private static LCapsuleWindow? QFootprintPlace(LCapsuleWindow? state, double left, double top, double width, double height, double minWidth, double minHeight)`

Clamps the stored size against the screen and the minimum size, keeping whether it was maximized.

## `private static LCapsuleWindow QFootprintClamp(LCapsuleWindow state, double left, double top, double width, double height, double placedWidth, double placedHeight)`

Clamps the stored position into the screen, once the size is settled.

## `public void QFootprintDefer(LCapsuleWindow state, bool minimized, bool closing)`

Hands the geometry to the posture.
A move or resize waits `QFootprintDelay` before writing, and a certain close writes at once.
The window view hands the restored rectangle, never the maximized one, so restoring returns to a usable size.
The minimized flag goes along, and the posture drops a minimized window.
An empty or degenerate rectangle is dropped, leaving the previous geometry in place.
