# PSLeave.xaml.cs

## `public partial class PSLeave : Window`

The leave dialog's answer, taken from whichever button closed it.
Closing it any other way answers cancel, so nothing is lost by a dismissed window.

## `internal static PSLeaveAnswer PSLeaveShow(Window owner)`

Shows the dialog over its owner and waits for the answer.
The owner centres it and holds still until it is answered.
