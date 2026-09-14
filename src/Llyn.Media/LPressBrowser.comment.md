# LPressBrowser.cs

## `public sealed class LPressBrowser`

Prints a rendered page, to PDF or to paper, through the browser engine the program already ships.
Printing the same HTML the export writes is what keeps the PDF, the paper and the page identical.
This lives outside the shell because it is a platform capability, not a panel.

## `private const double LPressBrowserMargin`

Twelve millimetres in inches, the same margin the page's own print rule declares.
The browser ignores that rule once it is handed margins, so the two must be kept equal by hand.

## `private static readonly string LPressBrowserFolder`

The temporary home of everything the press touches: the pages it prints and the browser's own profile.
Both sit under one folder so nothing of the press is scattered across the temp directory.

## `private static Task<CoreWebView2Environment>? _lPressSetting`

The one browser environment every print shares, kept as the task that made it.
Starting an environment spawns a browser process, which is too slow to repeat for each page.
Holding the task rather than the result lets a second print that arrives mid-start wait on the same one.

## `public async Task LPressSave(string html, string path)`

The page is written to a file rather than navigated to as a string.
A string navigation is capped in size, and an embedded picture passes that cap easily.
No dialog names a sheet for a file, so the regional sheet is used rather than the browser's Letter.
The temporary page is removed whether the print succeeded or failed.

## `public async Task LPressPrint(string html, LPressTicket ticket)`

The same page, sent to the printer the ticket names instead of a file.
The printer was chosen by the reader in the shell's dialog, and only that choice arrives here.
Every field of the ticket reaches the browser, so sides, collation and color come out as chosen.
A printer the engine cannot reach is reported as such, apart from any other failure.

## `private static CoreWebView2PrintSettings LPressSettingCreate(CoreWebView2Environment setting, LPressPaper paper)`

The sheet is set explicitly because the browser otherwise assumes Letter whatever the printer holds.
The margins are the page's own, restated here because the browser drops the page rule when given any.
They keep the text clear of the edge a printer cannot reach, where a zero margin would clip it.
Backgrounds are printed, since a card without its ground is not the card seen.

## `private static async Task LPressBoardRun(string html, Func<CoreWebView2Environment, CoreWebView2, Task> act)`

Opens a hidden browser on the page, runs `act` on it, and tears everything down again.
Both the file print and the paper print share this life, differing only in what `act` does.
The environment is fetched before the window opens, so a missing runtime leaves no window behind.

## `private static async Task<CoreWebView2Environment> LPressSettingRead()`

Hands back the shared environment, starting it on the first call.
A failed start is forgotten, so installing the runtime and printing again works without a restart.
A missing runtime is named as such, because the generic failure would not tell the reader what to install.

## `private static void LPressPageDelete(string page)`

Removes the temporary page once the print is over.
A page the browser still holds is left behind rather than allowed to mask the print's own outcome.

## `private static async Task LPressPageLoad(CoreWebView2 core, string page)`

Printing before the page settles would capture an unfinished document.
The handler is detached in every case, so a reused engine keeps no stale subscription.

## `private static IntPtr LPressWindowCreate()`

The browser engine needs a parent window even when nothing is shown.
The window is placed far off screen and never made visible, so no frame appears.
The caller's thread already pumps messages, which is what the engine needs.

## `private static void LPressWindowDispose(IntPtr frame)`

The window is destroyed after the controller closes, so nothing outlives one print.
