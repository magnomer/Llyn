# Shell

Tests of the console window, driven through `TInvocation.TInvocationRun` and the fake port.
A start with a terminal and no arguments is the window, and an F10 key ends it.

## `TWindow`

Each test queues any keys and sizes first, then runs the whole bootstrap.
The loop's own threads read the keys and sizes, so the tests assert the grid the fake rebuilt.

## `WindowOpen_EightyByTwentyFour_DrawsEmptyFrame`

Pins the whole frame at 80 by 24, with two bordered halves, the opening line and the quit legend.
The legend shows `10` and `Quit` in the last row at column 72.

## `WindowSizeObserve_Grown_RedrawsFrameByHalves`

Resizing to 64 by 18 and then to 101 by 30 ends on the large frame.
The odd width gives the left half 50 columns and the right half 51.

## `WindowSizeObserve_BelowEightyByTwentyFour_DrawsFramedNotice`

A console of 64 by 18 shows the small-console notice in a framed box instead of the frame.
The box is 44 cells wide, centred, with ` Llyn ` on its top border.
The notice wraps at a space within the 36-cell text and the quit legend sits on the last row.

## `WindowSizeObserve_KoreanNotice_WrapsByCells`

The Korean notice is on a 21 by 8 console, too small for the box, so it is bare text.
It wraps by cells, so a wide character never splits across lines.
A line shorter than the width ends in blanks, since a new canvas writes every cell.

## `WindowKeyObserve_FunctionTenUnderNotice_ExitsZero`

F10 ends the run with 0 even while the notice shows.
The window is left and nothing reaches stderr.

## `WindowBatchObserve_KeysWithoutChange_PrintsOneFrame`

Keys that change nothing print no further frame.

## `WindowKeyObserve_FunctionTen_ExitsZero`

A key other than a quit key is ignored, and F10 ends the run with 0.

## `WindowKeyObserve_ControlC_ExitsZero`

Ctrl+C arrives as a key, since the window makes it one, and ends the run with 0.

## `BootstrapIntroduce_WindowWorkspaceBusy_ExitsThree`

A busy workspace ends a window start with 3 and the three refusal lines on stderr.
No frame is printed and the window flag ends false.

## `BootstrapIntroduce_WindowFault_RestoresConsole`

A run seam that throws ends a window start with 4 after the window is left.
The last stderr line is the exception message.

## `BootstrapIntroduce_WindowRefusal_PrintsAfterRestore`

The trail shows the window restore first, then the refusal lines, then the mode restore.
The held lines wait for the alternate screen to close.

## `BootstrapIntroduce_WindowFault_PrintsAfterRestore`

The fault notice, the empty log line and the message follow the window restore in the trail.
The mode restore comes last.

## Stamp

Commit: `92c4426f8506`
- `TWindow.cs` `472441d419cdedab`
