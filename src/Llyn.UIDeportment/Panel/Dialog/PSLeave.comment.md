# PSLeave.cs

## `public class PSLeave`

The leave dialog's answer, taken from whichever button closed it.
Closing it any other way answers cancel, so nothing is lost by a dismissed window.
The window itself is the Veneer's markup, loaded and held in `_psLeaveSurface`.

## `internal PSLeave(Window owner)`

Loads the window, sets its owner and subscribes the three buttons.

## `private Button PSLeaveStore`

The named buttons are read through the window's name scope.

## `internal static PSLeaveAnswer PSLeaveShow(Window owner)`

Shows the dialog over its owner and waits for the answer.
The owner centres it and holds still until it is answered.
