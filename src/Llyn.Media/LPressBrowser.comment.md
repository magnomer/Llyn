# LPressBrowser.cs

## `public sealed class LPressBrowser`

Prints a rendered page to PDF through the browser engine the program already ships.
Printing the same HTML the export writes is what keeps the PDF and the page identical.
This lives outside the shell because it is a platform capability, not a panel.

## `public async Task LPressSave(string html, string path)`

The page is written to a file rather than navigated to as a string.
A string navigation is capped in size, and an embedded picture passes that cap easily.
The margins are zero because the page declares its own, and the browser must not add a second set.
Backgrounds are printed, since a card without its ground is not the card seen.
The temporary page is removed whether the print succeeded or failed.

## `private static async Task LPressPageLoad(CoreWebView2 core, string page)`

Printing before the page settles would capture an unfinished document.
The handler is detached in every case, so a reused engine keeps no stale subscription.

## `private static IntPtr LPressWindowCreate()`

The browser engine needs a parent window even when nothing is shown.
The window is placed far off screen and never made visible, so no frame appears.
The caller's thread already pumps messages, which is what the engine needs.

## `private static void LPressWindowDispose(IntPtr frame)`

The window is destroyed after the controller closes, so nothing outlives one print.
