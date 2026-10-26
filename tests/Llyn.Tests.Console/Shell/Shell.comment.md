# Shell

Tests of the console window, its envoy and its boxes, driven through `TInvocation.TInvocationRun` and the fake port.
A start with a terminal and no arguments is the window, and an F10 key ends it.
`TWindow` drives the window, `TBootstrap` the start boxes and `TEnvoy` the questions and notices.
`THelp` drives the help box.
`TWindowLayer` drives the roof menus, submenus, tab keys and boxes over the window.
`TCabinet` drives the `Options` menu, and `TEstablishment` the status line and the Joplin items.

## `TWindow`

Each test queues any keys and sizes first, then runs the whole bootstrap.
The loop's own threads read the keys and sizes, so the tests assert the grid the fake rebuilt.
`TInvocationRun` runs the bootstrap through `TConsoleFake.TConsoleFakeRun`, so a hung window fails the test on a time limit.

## `WindowOpen_EightyByTwentyFour_DrawsEmptyFrame`

Pins the whole frame at 80 by 24, with two bordered halves, the roof and the legend.
The status line below the halves reads `Joplin: Connect` and the tally, not the opening text.
The legend shows `1Help` at column 0, `9Menu` at column 64 and `10Quit` at column 72 in the last row.

## `WindowSizeObserve_Grown_RedrawsFrameByHalves`

Resizing to 64 by 18 and then to 101 by 30 ends on the large frame.
The odd width gives the left half 50 columns and the right half 51.

## `WindowSizeObserve_BelowEightyByTwentyFour_DrawsFramedNotice`

A console of 64 by 18 shows the small-console notice in a framed box instead of the frame.
The box is 42 cells wide, centred, with ` Llyn ` on its top border.
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

## `WindowKeyObserve_ShiftFunctionTen_KeepsWindow`

Shift+F10 is not the quit key, so the window stays until F10 ends the run with 0.

## `WindowKeyObserve_WideChosen_WidensMenuAndCanvas`

Choosing wide ambiguous characters in the `Options` menu makes the window count those characters as two cells.
The radio mark in the menu then takes the cells it needs, and the labels beside it still line up.
It runs on a fake port that also counts the characters as wide.
So the canvas and the menu are checked against one measure.

## `MuralDraw_LowerParleyField_ShowsNoCaret`

Only the top box shows a caret.
Two boxes with a field each give one `caret` text, and it is the top field's.
A box with no field above a field box leaves no caret at all.
So the terminal cursor never lands in a box that another box covers.

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
When the theme fails and the spare palette runs, the `Options` menu marks no theme.
The palette has no name.

## `TEnvoy`

Each test runs the envoy inside a window with the fake port, and queues the keys a user would press.
The helper `TEnvoyRun` also serves the other window tests.
It can prime the window, replace the theme loader and read the engine once the run settles.
It stores the language first, so a language choice can be seen in the stored value.
It waits for the window's exit for at most 30 seconds and asserts the exit happened.
So a lost key fails the test and does not hang it.
Enter answers Yes, Right then Enter answers No, and Escape and F10 answer No.
Shift with F10 leaves the box open, since a key matches its modifiers exactly.
A plain confirm draws an alert box, and a tally draws its count line.
A union confirm draws the dropped and kept names around an arrow.
A failure with only a headline draws no detail line.
A long path wraps at its separators, with the folder row and the file row.
A failure shown under a confirm draws above it, and the confirm still answers.
A fault box whose frame the console refuses reports to the run's own handler, so stderr holds that refusal.
The leave question answers store, discard or stay.
The coinage question answers the typed text, and Enter before typing waits for valid wording.
Tab moves focus to the buttons and back, so letters typed between are ignored.
The helper also takes the folder listing and drive seams that the path prompt reads.
They default to the disk.

## `THelp`

F1 opens help on the window page.
End scrolls it to show the links and the position `1 of 3`, one link for each other page.
F1 over a box opens the box page, with the box's keys listed.
F1 inside help shows the keys everywhere page.
Enter on a link follows it, and Backspace returns to the page before.
The tabs page names the Alt keys of the chooser, the history and the tab menu.
It names none of the quit keys.
Help over the tab chooser lists the chooser's keys, and its row wins where the box holds the same key.
After Tab moves the focus to the buttons, the same help names the button keys.
It then names none of the chooser's keys.
An open help changes the legend to `Close` only.
Help lists only keys that are bound and not hidden, and a dim key draws in the `disabled` role.

## `TWindowLayer`

The roof menus and the boxes over the window, one layer above the other.
A box over the window shows the Close legend, and queued work still runs under it.
F9 opens the product menu, and a box drawn over it covers it.
The product menu lists `Workspace…` under the Joplin items and above the first rule.
Enter on About closes the menu first, then shows the version, so the menu never stays under the box.
A box key pressed in an open menu, such as F1, F10 or Ctrl+C, closes the menu before it acts.
A submenu opens beside its item on Right, Enter or its hotkey, and Left or Escape closes only the submenu.
Up and Down move inside an open submenu, and choosing an item there runs it with the name it carries.
Alt+P opens the tab menu with rules between its groups.
Alt+F1 opens the chooser, and a digit picks a tab and closes it.
A hidden tab is left out of both the tab menu and the chooser.
With no history, Back and Forward draw dim, and stay dim in every frame.
A chosen tab shows its name at the end of the roof row.
Ctrl+C under a box answers the box's safe index.

## `TCabinet`

The `Options` menu, built directly and driven through the window.
The current theme, glyph set, ambiguous width and language each carry the radio mark.
A spare start theme has no name, so no theme is marked.
Choosing ASCII draws an ASCII frame, an ASCII notice and an ASCII arrow in the union question.
Choosing a language saves it through the engine and relabels the menu bar.
Choosing a theme redraws the whole window, and Settings selects the Settings tab.
Up inside a submenu moves inside it, and the theme loader is asked for the named theme.

## `TRoof`

The roof menu after a change of its items, and the product menu's workspace item.
When the lit item of an open submenu vanishes, the next live item is lit.
When it was the last, the first live item is lit.
The submenu stays open and draws only the items left.
The product menu joins `Workspace…` to the Joplin items above a rule, with the hotkey `W`.
Choosing it asks the envoy for the workspace, starting in the atelier's path.
Without a courier the item stands first, followed by a rule.

## `TEstablishment`

The status line and the Joplin items of the product menu.
Before anything is heard the line shows the opening text.
Without a stored token the line offers connecting, with the tally muted at the right.
With a token it shows connected.
The courier's line follows a separator until it is cleared.
A long one is cut with an ellipsis that keeps the right end.
With too little room the ellipsis alone stays, also when the first character is too wide to sit beside it.
The unsaved mark and count show only while changes are pending.
A newer establishment replaces the numbers, and drawing again shows the same row.
The product menu lists Connect only without a token, lists Sync in both cases, and sets them above a rule.
A busy courier dims both items and leaves About live.
Without a courier it leaves the Joplin items out.
Opening a workspace shows the line in the window.
The mirror reads the courier state when it subscribes, so the Joplin items show though no workspace opened.
A courier change keeps an open product menu open on its lit item, while Connect leaves it once attached.

## `TEstablishmentFixture`

The builders and readers the `TEstablishment` tests share.
It builds the status line, the product menu and a mirror on an 80 by 24 window.
Each takes the English catalog.
It reads a drawn row back as plain text padded to 80 cells.
The saved establishment and the idle courier state stay with the tests, since the tests read them directly.

## `TCapsule`

The console Capsule and the two owners that store their choices in it.
A saved state reads back equal, and no pending file stays behind.
A missing file, a file that does not parse and a null document read the default state.
A null root reads it too.
A save over a read-only file throws, keeps the old state and leaves no pending file.
Stored names this build does not know fall back to the defaults, and the stored file stays unchanged.
A stored theme the build lacks prints no theme notice at exit.
A workspace change mid-run draws the next box in the new workspace's stored glyph set.
Neither workspace's file is written by the change.
A theme, glyph set or width the new workspace's file lacks returns to its default, not the old workspace's value.
A name in the new workspace's file that this build does not know returns that choice to its default too.
A stored theme that fails to load on a workspace change shows no theme box.
A chosen theme that fails reports its reason and keeps the current theme.
Choices made in one wardrobe show in a new wardrobe over the same workspace, and the stored folder survives them.
Its Enter, Escape and F10 keys are shared with `TCapsulePathfinder`.

## Stamp

Commit: `c05429890cec`
- `TBootstrap.cs` `c4356b19e67b4c41`
- `TCabinet.cs` `ce20853cde2f2f7e`
- `TCapsule.cs` `334a49f71ae77378`
- `TEnvoy.cs` `ad12f1bf4da06bd6`
- `TEstablishment.cs` `13a9c3ba60eb79bc`
- `TEstablishmentFixture.cs` `d9e7172b48d235ca`
- `THelp.cs` `67309dcfe0c0bf71`
- `TRoof.cs` `d4ebea47cc21cee3`
- `TWindow.cs` `b561257947b5df5a`
- `TWindowLayer.cs` `5a7fe8f88f8468df`
