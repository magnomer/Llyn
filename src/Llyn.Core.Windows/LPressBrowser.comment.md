# LPressBrowser.cs
Hash: `0ec5161d6d11243a`

## `public sealed class LPressBrowser : LPress`

Saves a rendered page to PDF and opens print boards through the browser engine the program already ships.
Printing the same HTML the export writes is what keeps the PDF, the paper and the page identical.
This lives outside the shell because it is a platform capability, not a panel.

## `private const int LPressBrowserStyle`

The bare popup style of the hidden parent window, without the visible flag.
So the window never shows a frame or a caption.

## `private const int LPressBrowserWidth`

The width of the hidden window and browser, an A4 sheet at 150 dots per inch.
The page lays out at a sheet's proportions before it prints.

## `private const int LPressBrowserHeight`

The height of the hidden window and browser, the same A4 sheet at 150 dots per inch.

## `private static readonly string LPressBrowserFolder`

The temporary home of everything the press touches.
It holds the pages the press prints and the browser's own profile.
Both sit under one folder so nothing of the press is scattered across the temp directory.

## `private static Task<CoreWebView2Environment>? _lPressSetting`

The one browser environment every print shares, kept as the task that made it.
Starting an environment spawns a browser process, which is too slow to repeat for each page.
Holding the task rather than the result lets a second print that arrives mid-start wait on the same one.

## `public async Task LPressSave(string html, string path)`

Opens a board on the page, exports it through `LPressBoardExport`, and disposes it.
So a saved file and a print window's export share one browser path and one layout.
The page is written to a file rather than navigated to as a string.
A string navigation is capped in size, and an embedded picture passes that cap easily.
No dialog names a sheet for a file, so the setting a missing printer gets is used.
That setting carries the regional sheet, the default margin and the backgrounds, so exported files keep their layout.
The board's disposal removes the browser, its window and the temporary page, whether the export succeeded or failed.

## `public async Task<LPressBoard> LPressBoardOpen(string html)`

Opens one hidden browser on the page and hands it over as an `LPressController` that the caller disposes.
The page is loaded once, so each later proof skips the navigation and the wait for pictures and fonts.
A failed open removes the window and the page, since no board exists yet to dispose them.
It runs on the calling thread, because every browser call must stay on the thread that made it.

## `private static async Task<CoreWebView2Controller> LPressBoardLoad(CoreWebView2Environment setting, IntPtr frame, string page)`

Opens a browser in `frame`, sizes it to a sheet and loads `page` into it.
Every board opens its browser here, through `LPressBoardOpen`.
A browser whose page fails to load is closed before the failure travels on.
The window stays with the caller, who made it.

## `private static string LPressPageCreate(string html)`

Writes the page to a fresh file under the press folder and answers its path.
A fresh name per page keeps two open boards from overwriting each other.

## `internal static async Task<CoreWebView2Environment> LPressSettingRead()`

Hands back the shared environment, starting it on the first call.
A failed start is forgotten, so installing the runtime and printing again works without a restart.
A missing runtime is named as such, because the generic failure would not tell the reader what to install.

## `internal static void LPressPageDelete(string page)`

Removes the temporary page once the print is over.
`LPressController` removes its page here too, so the rule has one owner.
A page the browser still holds is left behind rather than allowed to mask the print's own outcome.

## `internal static async Task LPressPageLoad(CoreWebView2 core, string page)`

Printing before the page settles would capture an unfinished document.
Navigation can complete before pictures and fonts are ready, so the load also waits for both.
Each picture is made eager first, since a lazy one below the fold would never load.
A picture that fails to decode is skipped rather than holding the print forever.
The handler is detached in every case, so a reused engine keeps no stale subscription.

## `internal static IntPtr LPressWindowCreate()`

The browser engine needs a parent window even when nothing is shown.
The window is never made visible, so no frame appears.
It is owned by the foreground window and placed over it.
The print dialog opens over its owner, so it appears over the program rather than off screen.
With no foreground window it sits unowned at the screen's origin, still hidden.
The caller's thread already pumps messages, which is what the engine needs.

## `internal static void LPressWindowDispose(IntPtr frame)`

The window is destroyed after the controller closes, so nothing outlives its session.
`LPressController` destroys its own window here too when it is disposed.

## `private static extern IntPtr CreateWindowExW(int exStyle, string className, string? windowName, int style, int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param)`

The Win32 call that opens the hidden parent window.
The built-in `STATIC` class serves, so no window class is registered.

## `private static extern bool DestroyWindow(IntPtr window)`

The Win32 call that closes a parent window once its browser is closed.

## `private static extern IntPtr GetForegroundWindow()`

The Win32 call that names the window the print dialog should open over.

## `private static extern bool GetWindowRect(IntPtr window, [Out] int[] rect)`

The Win32 call that reads the owner's place, so the hidden window sits over it.
