# QSLeave.cs
Hash: `3e8093a00a96af04`

## `internal sealed class QSLeave`

The leave dialog's answer, taken from whichever button closed it.
Closing it any other way answers stay, so nothing is lost by a dismissed window.
The window is the veneer's `PSLeave` page, pulled fresh by contract ID and held in `_qsLeaveSurface`.

## `private QSLeave(Window owner)`

Pulls the window, sets its owner and subscribes the three buttons.
A dialog is its own window, outside the main window's look reach.
So it attaches the look to itself, or its buttons show no label and no fill.

## `private Button QSLeaveStore`

Each named button is pulled from the window by its contract ID.

## `internal static QSLeaveAnswer QSLeaveShow(Window owner)`

Shows the dialog over its owner and waits for the answer.
The owner centres it and holds still until it is answered.
