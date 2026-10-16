# Llyn.Core.Windows

Core.Windows is Core's Windows twin, holding the platform capabilities the logic rings may not reference.
No logic ring may reference it.
Only the composition root and Host name its classes.

## `Llyn.Core.Windows.csproj`

The Windows version in the target makes the system PDF reader in `Windows.Data.Pdf` visible.
Host, Deportment and Veneer already target the same version, so no project above needs a new one.
The WebView2 package lives here because the browser engine prints, proofs and saves every press page.
DPAPI, which protects the Joplin token, comes with the Windows desktop framework, so no package brings it.

## `LPhonographMedia`

The phonograph the composition root hands the engine.
It holds one platform player and maps each port call onto it, with no logic of its own.
The root builds one for the whole run, so a workspace switch keeps the same player.
The player belongs to the shell's thread, so a call from another thread is sent there first.
Stopping closes the player, so the file is no longer held open.

## `LPhonographFailHandle`

Closes the player when a file fails to play, so the file is released.
The failure is not reported, since the phonograph has no route to the window.
It then raises `LPhonographFinished`, so a run of sounds goes on to its next file.

## `LPhonographFinishHandle`

Raises `LPhonographFinished` when the player reaches the end of a file.

## `LPressBrowser`

Saves a rendered page to PDF and opens print boards through the browser engine the program already ships.
Printing the same HTML the export writes keeps the PDF, the paper and the page identical.
This lives outside the shell because it is a platform capability, not a panel.

## `LPressBrowserFolder`

The temporary home of everything the press touches.
It holds the pages the press prints and the browser's own profile.
Both sit under one folder so nothing of the press is scattered across the temp directory.

## `LPressBrowserStyle`

The hidden parent window uses a bare popup style without the visible flag.
So the window never shows a frame or a caption.

## `LPressBrowserWidth`

The hidden window and browser are an A4 sheet wide at 150 dots per inch.
`LPressBrowserHeight` gives the same sheet's height.
The page lays out at a sheet's proportions before it prints.

## `_lPressSetting`

The one browser environment every print shares, kept as the task that made it.
Starting an environment spawns a browser process, which is too slow to repeat for each page.
Holding the task rather than the result lets a second print that arrives mid-start wait on the same one.

## `LPressSave`

Opens a board on the page, exports it through `LPressBoardExport`, and disposes it.
So a saved file and a print window's export share one browser path and one layout.
The page is written to a file rather than navigated to as a string.
A string navigation is capped in size, and an embedded picture passes that cap easily.
No dialog names a sheet for a file, so the setting a missing printer gets is used.
That setting carries the regional sheet, the default margin and the backgrounds, so exported files keep their layout.
The board's disposal removes the browser, its window and the temporary page, whether the export succeeded or failed.

## `LPressBoardOpen`

Opens one hidden browser on the page and hands it over as an `LPressController` that the caller disposes.
The page is loaded once, so each later proof skips the navigation and the wait for pictures and fonts.
A failed open removes the window and the page, since no board exists yet to dispose them.
It runs on the calling thread, because every browser call must stay on the thread that made it.

## `LPressBoardLoad`

Opens a browser in the given window, sizes it to a sheet and loads the page into it.
A browser whose page fails to load is closed before the failure travels on.
The window stays with the caller, who made it.

## `LPressPageCreate`

Writes the page to a fresh file under the press folder and answers its path.
A fresh name per page keeps two open boards from overwriting each other.

## `LPressSettingRead`

Hands back the shared environment, starting it on the first call.
A failed start is forgotten, so installing the runtime and printing again works without a restart.
A missing runtime is named as such, because the generic failure would not tell the reader what to install.

## `LPressPageDelete`

Removes the temporary page once the print is over.
`LPressController` removes its page here too, so the rule has one owner.
A page the browser still holds is left behind rather than allowed to mask the print's own outcome.

## `LPressPageLoad`

Printing before the page settles would capture an unfinished document.
Navigation can complete before pictures and fonts are ready, so the load also waits for both.
Each picture is made eager first, since a lazy one below the fold would never load.
A picture that fails to decode is skipped rather than holding the print forever.
The handler is detached in every case, so a reused engine keeps no stale subscription.

## `LPressWindowCreate`

The browser engine needs a parent window even when nothing is shown.
The window is never made visible, so no frame appears.
It is owned by the foreground window and placed over it.
The print dialog opens over its owner, so it appears over the program rather than off screen.
With no foreground window it sits unowned at the screen's origin, still hidden.
The caller's thread already pumps messages, which is what the engine needs.
`CreateWindowExW` uses the built-in `STATIC` class, so no window class is registered.

## `LPressWindowDispose`

The window is destroyed after the controller closes, so nothing outlives its session.
`LPressController` destroys its own window here too when it is disposed.

## `LPressController`

One hidden browser kept open on one sheet for the life of a print window.
Each proof prints the loaded page again instead of opening a new browser.
Nothing is kept globally, so disposing the board releases everything it holds.
The mapping from setting to browser print settings lives here as private helpers, since only the board prints.
The controller takes ownership of a window, a browser and a page that `LPressBoardOpen` already loaded.

## `_lPressControllerFrame`

The hidden parent window the browser lives in.
It is destroyed only after the browser closes.

## `_lPressControllerCore`

The browser that holds the loaded page.
It turns null once the board is disposed, which makes a second dispose do nothing.

## `_lPressControllerTurn`

The turn every browser print operation takes.
The browser runs one print operation at a time and throws on a second.
Disposing also waits for the turn, so a proof in flight finishes before the browser closes.

## `LPressBoardDraw`

Prints the loaded page to a PDF stream and opens it as a proof.
The stream belongs to the proof from then on.
It continues on the calling thread, because every browser call must stay on the thread that made the browser.
A disposed board raises `ObjectDisposedException`.

## `LPressBoardPrint`

Prints the loaded page to the chosen printer as vector output.
It starts from the same layout settings as the proof and only adds the printer keys.
So the paper matches the preview page for page.
An unknown or offline printer answers `LPressStatusOffline`.
Any other browser failure answers `LPressStatusError`.
Settings the browser rejects raise `ArgumentException`, which is caught and answers `LPressStatusError`.
Save as PDF has no printer, so asking it to print raises `InvalidOperationException`.
It holds the turn, since the browser runs one print operation at a time.

## `LPressBoardExport`

Saves the loaded page as a PDF at the path from the proof's layout settings.
The page range is added, since only the proof must always hold every page.
The printer keys are left out because the PDF methods ignore them.
A false answer from the browser raises `IOException`, which `LPressSave` passes on.
It holds the turn like every other print operation.

## `LPressBoardShow`

Opens the system print dialog on the loaded page as the fallback for driver-only features.
Stapling, trays and finishing exist only there.
It holds the turn until the dialog closes, so no proof starts while the dialog prints.

## `LPressDialogShow`

Shows the system print dialog and waits until it closes.
The browser gives no completion for this dialog, so the page's `afterprint` event stands in for one.
The listener is in place before the dialog opens, so a quick cancel is not missed.
A cancelled dialog ends the wait the same way, since the page fires `afterprint` either way.

## `LPressPrintingCreate`

Turns a press setting into the browser's layout settings.
The proof, the export and the print all start from these layout settings.
So a preview, its file and its paper never lay out differently.
It maps only the layout keys, which are paper, orientation, margins, scale and backgrounds.
The printer, copies, range, colour and sides do not move the layout, so a proof never carries them.
The sheet is set explicitly because the browser otherwise assumes Letter whatever the printer holds.
A custom sheet takes the setting's width and height, as the `MediaSize` remarks advise.
The documentation does not say whether `Landscape` swaps `PageWidth` and `PageHeight`.
Nothing there contradicts sending upright dimensions with the orientation alone turning the sheet.
Paper always travels in portrait dimensions, so a sheet listed wide by a driver cannot be turned twice.
The proof and the print share that rule, so both turn the sheet the same way.
The margins are restated because the browser drops the page's own print rule when given any.
The scale is a percent in the setting and a factor in the browser.
The setting arrives already clamped, so no value here can be rejected by the browser.

## `LPressPrintingApply`

Adds the printer-only keys to the layout settings, for printing alone.
These are the printer, copies, collation, colour, sides, page range and a custom sheet.
None of them moves the layout, so the print still matches the proof.
Each `Default` member maps to the browser's `Default`, which leaves the choice to the printer.
The sheet is sent as custom so the printer takes the setting's width and height.
With the browser's default sheet, the printer's own paper would win over the preview's.
Save as PDF carries no printer, so applying it raises `InvalidOperationException`.

## `DisposeAsync`

The controller waits for the turn, then closes the browser, destroys its window and deletes its page.
The browser closes before its window, since the window is its parent.

## `LPressDocument`

One rendered PDF of the sheet, drawn to page images by the system PDF reader.
The preview shows these images, so it shows exactly what the browser printed.
The constructor is reached only through `LPressDocumentOpen`, so no proof exists without a loaded PDF.
`LPressProofCount` is the page count the PDF reports, known without drawing any page.

## `_lPressDocumentStream`

A seekable copy of the PDF the browser printed.
The reader reads from it while drawing, so it lives as long as the proof.

## `LPressDocumentOpen`

Copies the stream into memory, disposes it, and loads the PDF from the copy.
The browser's stream cannot seek, and the PDF reader needs one that can.
On any failure both streams are disposed and the exception rethrows, since no proof exists to dispose them later.

## `LPressProofRead`

Answers the page's own size in inches, read from the PDF.
It is never computed from the setting, so rounding cannot make the view and the PDF differ.
The reader measures in units of a ninety-sixth of an inch.
Pages count from zero.

## `LPressProofDraw`

Draws one page as a PNG at the resolution and answers the bytes.
The page size in device-independent units is 96 per inch, so both sides scale by `resolution / 96`.
Each side keeps at least one pixel.
It may run off the UI thread, because the PDF reader has no thread affinity.
Dispose releases the PDF copy the reader draws from.

## `LPressSpooler`

Reads the installed printers and what each one can do, as plain Core records.
It is Core's Windows twin of `LPressQueue`, so no layer above sees System.Printing.
Only Host names it, and hands it to the rig as `LRigQueue`.

## `LPressQueueRead`

Runs the whole read on one fresh background STA thread and answers when it ends.
System.Printing objects must stay on the thread that made them.
A dead network queue can block for seconds, so the read never runs on the caller's thread.
Both local queues and connected network queues are listed.
The default printer is marked by its full name, which is also the name the browser prints to.
A machine without a default printer still lists its printers, just with none marked.
A failed enumeration faults the answer with its own exception.
The print window reports it and still offers Save as PDF.
The continuation runs asynchronously, so the caller never resumes on the worker thread.

## `LPressSpoolerScan`

Builds one printer record from the queue's reported capabilities.
A queue that cannot report them is kept as unknown, with empty lists and no copy limit.
So a broken driver still offers its printer with the standard sheets and printer defaults.
Sheet sizes arrive in 1/96 inch and are handed on in inches, as the driver lists them.
Sheets without a size are dropped, since a missing size has no Core value.
Turning sheets upright, dropping repeats and naming standard sheets is a rule, so Application does it.
`LPortraitClerkPress.LPortraitPrinterRead` applies it to the whole list.
Monochrome counts as gray, since the browser offers only one non-colour mode.
Collation is offered only when the driver reports collated output.

## `LUsherShell`

The usher the composition root hands the engine.
Path facts go to the inner usher, and the launch runs here.
This lives beside the press because starting a process is a platform capability, not a file.
The veneer never starts a process itself, so every launch passes through the engine to here.
`LUsherOpen` hands the target to the operating-system shell.
The shell picks the file manager for a folder and the default browser for an address.
A launch that fails throws, and the caller shows the failure.

## `LWarrantShield`

The Windows adapter that protects the Joplin token with DPAPI for the current user.
Only the same user can read the stored text back.
A roaming Windows profile may recover the token on another machine, as `LWarrant.comment.md` says.

## `LWarrantShieldSalt`

A fixed entropy that separates Llyn's protected tokens from other programs' data.
It does not stop a process running as the same user.

## `LWarrantHide`

Refuses a blank token, since there is nothing worth protecting.
The result is Base64 so settings can keep it as plain text.
A `CryptographicException` from Protect escapes, so the caller reports it.

## `LWarrantRestore`

Answers null for every unrecoverable input instead of throwing.
A caller then asks Joplin for a fresh token rather than failing on a stale setting.

## Stamp

Commit: `bd299c638b2c`
- `LPhonographMedia.cs` `4f48ad7e405721fd`
- `LPressBrowser.cs` `0ec5161d6d11243a`
- `LPressController.cs` `cc054b88aa9d403b`
- `LPressDocument.cs` `108ca38d745071a2`
- `LPressSpooler.cs` `b4c79568a7583a71`
- `LUsherShell.cs` `02ab055fa7b0f437`
- `LWarrantShield.cs` `fae5bf15c3da86b0`
- `Llyn.Core.Windows.csproj` `52f86fffcc07ada1`
