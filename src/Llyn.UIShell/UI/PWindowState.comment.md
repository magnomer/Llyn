# PWindowState.cs

## `public partial class PWindow`

Where the window was and how big it was, carried from one run to the next.
The workspace settings file holds it, so a workspace opens the way its owner left it.

## `private void PWindowStateRestore()`

Runs before the window is shown, while position and size can still be set.
Stored geometry is clamped into the current virtual screen and against the minimum size.
A monitor that is gone, or a saved size larger than the desktop, must not leave the window unreachable.
Setting the startup location to manual is required, since the designed default centres the window.

## `private void PWindowStateSave()`

Runs on closing, once the user has confirmed and the window is still up.
The restored rectangle is stored, never the maximized one, so restoring returns to a usable size.
`RestoreBounds` supplies it whenever the window is maximized or minimized.
An empty or degenerate rectangle is dropped, leaving the previous geometry in place.
