# Shell

Tests of the console window, its envoy and its boxes, driven through `TInvocation.TInvocationRun` and the fake port.
A start with a terminal and no arguments is the window, and an F10 key ends it.
`TWindow` drives the window, `TBootstrap` the start boxes and `TEnvoy` the questions and notices.

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

## `BootstrapFaultObserve_WindowRunning_DrawsFaultBox`

A fault posted to the loop draws a fault box with the message, and nothing reaches stderr.
The run still exits 0.

## `BootstrapIntroduce_WindowWorkspaceBusy_ExitsThree`

A busy workspace ends a window start with 3 and the three refusal lines on stderr.
The refusal box is drawn first, so a frame is printed, and the window flag ends false.

## `BootstrapIntroduce_WindowFault_RestoresConsole`

A run seam that throws ends a window start with 4 after the window is left.
The last stderr line is the exception message.

## `BootstrapIntroduce_WindowRefusal_PrintsAfterRestore`

The trail shows the window restore first, then the refusal lines, then the mode restore.
The held lines wait for the alternate screen to close.

## `BootstrapIntroduce_WindowFault_PrintsAfterRestore`

The fault notice, the empty log line and the message follow the window restore in the trail.
The mode restore comes last.

## `TBootstrap`

A refused workspace in the window mode draws an alert box with the path and the reason.
The box has an OK button, and Enter or F10 ends the start with 3.
The refusal lines print on stderr after the window is left.
A rescue draws an information box with the backup path and the reason.
A rescue and a failed theme both draw, with the rescue first.

## `TEnvoy`

Each test runs the envoy inside a window with the fake port, and queues the keys a user would press.
Enter answers Yes, Right then Enter answers No, and Escape and F10 answer No.
A plain confirm draws an alert box, and a tally draws its count line.
A union confirm draws the dropped and kept names around an arrow.
A failure with only a headline draws no detail line.
A long path wraps at its separators, with the folder row and the file row.
A failure shown under a confirm draws above it, and the confirm still answers.
A fault box whose frame the console refuses reports to the run's own handler, so stderr holds that refusal.
Work queued while a box is open still runs.
A box changes the legend to `Close`.
Ctrl+C under a box ends the window, and the box answers its safe index.
The leave question answers store, discard or stay.
The coinage question answers the typed text, and Enter before typing waits for valid wording.
Tab moves focus to the buttons and back, so letters typed between are ignored.

## Stamp

Commit: `9a85a3ab82db`
- `TBootstrap.cs` `2cf3b16c8595d74e`
- `TEnvoy.cs` `bbfbe6beda796227`
- `TWindow.cs` `2bb9db2b4111cbe6`
