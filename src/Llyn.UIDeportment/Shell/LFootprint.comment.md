# LFootprint.cs

## `public sealed class LFootprint`

Where the window was and how big it was, carried from one run to the next.
The window's posture holds it, so a workspace opens the way its owner left it.

## `private const double LFootprintShare = 0.8;`

The share of the work area a first launch fills.
A designed pixel size cannot fit every desktop, so the default is measured against the screen.

## `private const int LFootprintDelay = 700;`

The rest in milliseconds the posture waits after a move before it writes.
A drag raises a change for every pixel, and one write per pixel would be a file write per pixel.

## `public LFootprint(Window window, LWindow deportment)`

Listens for the window only once it is loaded.
Nothing is written before then, so the restore does not save what it has just read.

## `public void LFootprintRestore()`

Runs before the window is shown, while position and size can still be set.
With nothing stored the window takes its share of the work area and stays centred.
Stored geometry is clamped into the current virtual screen and against the minimum size.
A monitor that is gone, or a saved size larger than the desktop, must not leave the window unreachable.
Setting the startup location to manual is required, since the designed default centres the window.

## `public void LFootprintClosingHandle(CancelEventArgs e, bool confirmed)`

Declining leaves the window open on the form exactly as typed, which is the only place the work still exists.
Geometry is stored only once the close is certain, so a declined close changes nothing.
The close writes at once, with no delay.

## `private void LFootprintAttach()`

Listens for the window being moved, resized or maximized.
Geometry is written down while the program runs rather than only as it closes.
A run that ends without closing used to leave the last geometry unwritten.
A killed process, a launcher window shut and a machine turned off all end that way.

## `private void LFootprintSave(int delay)`

Hands the window's geometry to the posture, which waits `delay` milliseconds before writing.
The restored rectangle is stored, never the maximized one, so restoring returns to a usable size.
`RestoreBounds` supplies it whenever the window is maximized.
The minimized flag goes along, and the posture drops a minimized window.
An empty or degenerate rectangle is dropped, leaving the previous geometry in place.
