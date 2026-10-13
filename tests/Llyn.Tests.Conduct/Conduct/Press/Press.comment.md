# Press

This folder holds the print window gates.
They cover choice, delay, lens, margin, memory, output, printer, proof and range.

## `TPressWindowChoice`

Covers the pickers of the print window, their ready choice lists and their index gates.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.
A fresh setting lists the pages, margin and two-sided rows in display order, with one row chosen.
The driver fills each picker from that list alone, so the order lives only in the setting.
Each index gate maps a listed row number to the value that row carries.
A row number of -1 or past the list changes nothing.
Every index gate raises `CSPressSettingChanged` exactly once, whether it changes the setting or not.
Save as PDF lists no two-sided rows, so its index gate leaves the field null.
The printer index gate keeps its own facts in `TPressWindowPrinter`.

## `TPressWindowDelay`

Covers the galley's deferred redraw and its page window.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, which pins the wait to `Timeout.Infinite`.
A fact sets the wait to zero through `TPressDelaySet` before the request it checks.
So nothing fires by the clock, and every event comes from the request under test.
A stale edit raises one state event carrying the drawn setting.
An edit left pending is replaced by a newer one, so one redraw renders the newer scale.
A failed deferred redraw shows `Print.RenderFailed` through the ledger and raises the failure state.
A sight raises one page event per page of its window, each a PNG with its page index.
A sight before any draw raises nothing and shows nothing, since no proof is held.
The window is the visible range and one page either side.
A repeated sight at the same resolution requests nothing, since those pages were delivered.
The same sight after a zoom past the default detail delivers its pages again and drops none.
A redraw forgets the delivered pages, so the same sight requests them again.
A sight that fails on its first page shows `Print.RenderFailed` once and delivers nothing.
After the galley is disposed, neither an edit nor a sight raises an event or reaches the board.
A sight also carries the page a reader is on, and a new page becomes the current page.
The same page again raises no setting, so a scroll within one page stays quiet.
A sight that moves the window raises a drop event for each delivered page it leaves.
A redraw raises a drop event for every delivered page, in page order.

## `TPressWindowLens`

Covers the print preview's zoom, owned by the galley's `CSLens`.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen` and drives the lens through its relays.
A fact needing the paper size draws a proof first and reads the size from the answer.
Typed 150 with the culture's percent sign gives a free factor of 1.5.
The answered text is that factor as the culture writes a percent.
Typed 75 with no sign gives a free factor of 0.75.
Typed text that is no number refits in the current mode, here a width fit.
From 100 percent, a raw wheel delta up steps to 110 and a step up then to 125.
From 80 percent, which lies between rungs, a step down lands on 75 and keeps the free mode.
A typed 1000 clamps to 400 percent, and a step up stays there.
A typed 1 clamps to 10 percent, and a step down stays there.
A width fit answers the view width over the paper width.
A page fit answers the smaller of the width and height ratios.
At 400 percent in a wide view, the sight asks for more than the default 300 dots per inch.
The delivered PNG is wider than the paper at 300 dots per inch, read from its header.

## `TPressWindowMargin`

Covers the four margin fields, each of which hands its own typed text to its own gate.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.
Each edge gate sets its edge from the text and takes the other three from the setting last answered.
So a typed edge changes that edge only, and the margin stays Custom.
Each edge gate raises `CSPressSettingChanged` exactly once.
The four-value gate and the presets keep their facts in `TPressWindowPrinter`.

## `TPressWindowMemory`

Covers the setting the window keeps through the store and restores in the next window.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`.
A setting the first window saved on closing restores over a proof drawn with another layout.
The restore drops the typed range and makes the proof stale.
An empty store keeps the setting.
Unreadable text is the store's concern, checked in `TPressFile`.
The restore policy for unlisted printers is checked in `TPressClerk`.

## `TPressWindowOutput`

Covers the print, the export and the busy state they report.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`.
A print hands the setting to the board and asks nothing.
The system gate shows the board's own dialog.
An offline or failing printer returns its wording key.
Save as PDF asks for a file, and a declined file exports nothing.
A held print reports busy through the setting event, then idle once it ends.
A second print while one is held is refused with `Print.Printing`.
A close while a print is held is refused and saves nothing.
A close when idle saves the setting once.
A store that cannot be written shows `Print.Failed`, and the window still closes.

## `TPressWindowPrinter`

Covers the printer choice and the fields it governs.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`.
A fresh setting lists Save as PDF first and the default printer chosen.
A spooler that fails still opens the window, on Save as PDF alone.
The failure reaches the ledger as `Print.Failed`.
Save as PDF hides copies, collation, colour, sides, sheets and the hint.
An index of -1 or past the list keeps the chosen printer.
Copies clamp to the printer's maximum and to one, and an unreported maximum clamps to 999.
Scale clamps to 10 and 200.
A text that is no number keeps the old copies and scale.
Collation shows only with several copies.
A printer that reported nothing falls back to the standard papers and shows the capability hint.
It also hides collation, and its colour and sides fall back to full and single.
A reporting printer keeps gray and long edge, but an unreported short edge falls back to single.
Typed margins store a custom margin, and typed zeros pick the None preset.
A margin text that is no number keeps the old margin.

## `TPressWindowProof`

Covers the proof the window draws from the setting.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`.
Only orientation, paper, margin, scale and background make the proof stale, and a draw clears it.
An unchanged setting skips the second render, and a changed one renders the latest setting.
A first draw answers three pages at the paper size times 96, and landscape swaps width and height.
A draw held open while a newer setting arrives answers null.
The page draw has no gate of its own, since only the sight pass draws pages.
Its PNG and page index are held by `TPressWindowDelay`, and its resolution and range by `TGalleyClerk`.
A board whose draw fails returns the failure state for the retry veil and shows the same key.
The press stays stale, but the failure state's setting does not, so no busy veil shows.

## `TPressWindowRange`

Covers which pages print and how many sheets they take.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`.
Before a draw the setting lists no pages and is stale.
A page outside the proof, a reversed span or text with no number reports `Print.PagesInvalid`.
The typed text stays, and every page stays included.
The Current range follows the selected page, and a typed page number clamps to the proof.
Text typed alone keeps the chosen mode and raises the setting once.
Sheets halve for two-sided printing and multiply by copies.
Exactly one sheet picks the singular wording key.

## Stamp

Commit: `c2f335c331fe`
- `TPressWindowChoice.cs` `e65a79eaf2bf0625`
- `TPressWindowDelay.cs` `04061a6e817809cb`
- `TPressWindowLens.cs` `6c39b5bb1931430c`
- `TPressWindowMargin.cs` `cda21723c8181c0c`
- `TPressWindowMemory.cs` `bb3ffcc626e253a5`
- `TPressWindowOutput.cs` `ed6d2f9405989c4e`
- `TPressWindowPrinter.cs` `f9badfbe67cfb7c2`
- `TPressWindowProof.cs` `d8b5763f3a09e514`
- `TPressWindowRange.cs` `ba7d94550c422f51`
