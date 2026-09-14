# PWindowState.cs

## `public partial class PWindow`

Where the window was and how big it was, carried from one run to the next.
The workspace settings file holds it, so a workspace opens the way its owner left it.

## `private const double PWindowStateShare = 0.8;`

The share of the work area a first launch fills.
A designed pixel size cannot fit every desktop, so the default is measured against the screen.

## `private void PWindowStateAttach()`

Listens for the window being moved, resized or maximized.
Geometry is written down while the program runs rather than only as it closes.
A run that ends without closing used to leave the last geometry unwritten.
A killed process, a launcher window shut and a machine turned off all end that way.

## `private void PWindowStateDefer()`

Restarts the short wait that stands between a change and the write it causes.
A drag raises a change for every pixel, and one write per pixel would be a file write per pixel.
Nothing is written before the window is loaded, so the restore does not save what it has just read.

## `private void PWindowStateRestore()`

Runs before the window is shown, while position and size can still be set.
With nothing stored the window takes its share of the work area and stays centred.
Stored geometry is clamped into the current virtual screen and against the minimum size.
A monitor that is gone, or a saved size larger than the desktop, must not leave the window unreachable.
Setting the startup location to manual is required, since the designed default centres the window.

## `private void PWindowStateSave()`

Runs on closing, once the user has confirmed and the window is still up.
Runs again whenever the window has come to rest after a move.
The geometry alone is handed downstream, never the whole settings record.
A language chosen during the same session therefore survives the close.
The restored rectangle is stored, never the maximized one, so restoring returns to a usable size.
`RestoreBounds` supplies it whenever the window is maximized.
A minimized window saves nothing, so the maximized flag it had before survives a close from the taskbar.
An empty or degenerate rectangle is dropped, leaving the previous geometry in place.
