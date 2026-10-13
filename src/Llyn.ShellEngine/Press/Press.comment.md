# Press

The print session of the engine, one galley and relay pair per print window.
The galley owns the setting, the relay draws and prints it, and the records carry facts to the gate.
It stands apart from Engine because the board's browser is bound to the thread that opened it.
So nothing here hops threads, and the session is used from that one thread.

## `LGalley`

The engine's setting session for one print window, from the window opening to its close.
It keeps the printers read when the window shows, the setting the reader is shaping, its scope and typed range.
Every option edit is clamped through `LPressClerk` against the chosen printer, so Conduct mirrors no print data.
It keeps no window state, so the page the canvas shows and the running flag live in the gate.
The galley names no Core rule, and holds Core records only as data.
A gate holds the galley as a panel holds a tenure, and reads ready answers from it.
The galley draws, prints and disposes nothing.
`LGalleyRelay` does that, and holds this galley as the setting it draws and prints.
The galley reads the proof's page count from the session clerk, never its board or proof.
The relay owns the session clerk and disposes it, so the galley only reads it.
The press clerk lists printers through Application, since ShellEngine names no Core port.
The print setting clerk owns every setting rule and the kept setting, and the galley keeps the session state.
The session opens with no printers, on Save as PDF.
The file name is the one a PDF save is offered under, formatted by the settings facade.

## `LGalleySetting`

The current setting, always clamped for the chosen printer.
Each edit replaces it whole, so a draw in flight keeps the setting it started with.
`LGalleyPrinter` lists the printers the last restore found, in the spooler's order.
It stays empty when listing failed, so only Save as PDF is offered.

## `LGalleyRange`

The page range text as the reader last typed it, kept even when invalid.
A new proof can change the page count, so the text is parsed again after each draw.
`LGalleyScope` is the pages mode the reader chose, which decides the range text.
It opens on All, matching the empty range text.
`LGalleyRangeValid` says whether the typed text names pages the held proof has.
Before the first draw only the form is checked, as in `LGalleyRangeSet`.

## `LGalleyStale`

It says whether the held proof no longer shows the setting's layout, or no proof exists yet.
The clerk holds the setting the proof was drawn with, so it answers this.
Copies, range, colour and sides never make it stale.
A printer change makes it stale only when the clamp changes the layout.
`LGalleyCount` is the page count of the held proof, or null before the first draw.
The clerk owns the proof, so the galley reads the count and never stores it.

## `LGalleyMetric`

It says whether custom margins are typed in millimetres, read from the region once at open.
`LGalleyMargin` gives the setting's margin with its values in that unit.
It is the reverse of the four-value `LGalleyMarginSet`, so typed values round-trip.
Its name still marks a preset, so only the numbers change.

## `LGalleyDigest`

The ready facts the view shows, read fresh from `LPressClerk.LPressClerkRead`.
It passes the drawn page count, or null before the first draw.
`LGalleyPaperSelect` indexes into its offered sheets.

## `LGalleySave`

It keeps the live setting in the store through `LPressClerk.LPressClerkSave`.
A store that cannot be written raises, and the gate reports it.

## `LGalleyRestore`

It lists the printers, applies the kept setting to the live one and answers the ready view.
Restoring needs the printers, since a remembered printer is matched by name.
The kept setting is read and applied in a `finally`, so a listing failure still restores against no printers.
The failure then travels on to the gate, which reports it.
No `ConfigureAwait(false)` is used, so the setting changes on the window's thread.
The private overload applies the setting the clerk fitted to the listed printers.
Null means nothing is kept, which leaves an edited setting untouched.
A setting nobody edited yet takes the clerk's opening setting for the listed printers.
A kept setting resets the scope to all pages and clears the typed range.
The next draw renders when the restored layout differs from the held proof.

## `LGalleyPrinterSelect`

It chooses the printer by queue name, or Save as PDF when the name is null.
`LPressClerk.LPressClerkSelect` clamps against the new printer, so options it lacks fall back.
A name outside `LGalleyPrinter` is a caller mistake and throws.

## `LGalleyCopiesSet`

It sets the copy count the reader typed, clamped to the printer's limit.
Text that is not a whole number leaves the setting untouched, so a half-typed field moves nothing.
Collation stays off for a printer that cannot collate.

## `LGalleyRangeSet`

The public form chooses the pages mode and sets the range text it stands for.
All sets empty text, Current sets `page`, and Custom sets `text`.
`page` is the canvas's page counted from zero, which the gate holds.
It answers whether the resulting text names pages that exist.
The private text form keeps the text and answers the same.
Before the first draw the page count is unknown, so only the form is checked.
An invalid text leaves the setting's last valid range in place.
The private page form sets the range to one page counted from zero like the proof.
Range text counts from one, so the page is written one higher.
The parameterless form parses the typed range again against the page count of the proof just drawn.
`LGalleyRelay` calls it after each draw that a newer one did not supersede.
A range that does not parse leaves the setting's range as it was.

## `LGalleyPageSelect`

The index form applies a new canvas page, counted from zero, to the range.
The gate keeps the page, so the galley only rewrites a Current range to it.
Paging never moves another scope's range.
A negative page is a caller mistake and throws.
The text form answers the page whose number the reader typed, counted from one as a reader counts.
The current page is answered back when nothing changes.
A number past either end selects the nearest page, so a typed page always lands somewhere.
Text that is no whole number, or no drawn proof with pages, leaves the page untouched.
A changed page goes through the index form, so a Current range follows it.

## `LGalleyOrientationSet`

It sets the orientation, which makes the proof stale.
The colour and side sets fall back to the printer default when the printer lacks the mode.
The background set decides whether background colours and pictures print.

## `LGalleyPaperSelect`

It chooses a sheet by its place in the offered sheets of `LGalleyDigest`.
An index outside that list is a caller mistake and throws.

## `LGalleyMarginSet`

The record form sets a preset margin, or any margin already in inches.
A margin that leaves no room on the sheet falls back to the default.
The name form chooses a preset through `LPressClerk.LPressClerkResolve`.
A null name keeps the current values as a custom margin, so choosing Custom moves nothing.
A name outside the presets is a caller mistake and throws.
The four-text form sets custom margins typed in the region's unit, which `LGalleyMetric` names.
The text is read in the current culture, the same culture the fields are shown in.
If any of the four is not a number, the setting stays untouched.
Values that equal a preset resolve to that preset.

## `LGalleyScaleSet`

It sets the scale percent the reader typed, clamped to the range the browser accepts.
Text that is not a whole number leaves the setting untouched.

## `LGalleyDestination`

One place a print can go, as the galley view lists it.
It carries facts only, so the gate chooses the wording it shows.
The name is the printer's queue name, or null for Save as PDF.
The record also says whether the printer is the system default and whether the setting prints here.

## `LGalleyProof`

The relay's answer to a proof draw that was not superseded.
The canvas lays its sheets out from it before any page is drawn.
It carries the page count and the first page's size in device-independent pixels, 96 to the inch.
It also carries the dots per inch a page is drawn at when no resolution is asked.
Conduct's lens asks for a higher one only when the zoom needs more detail than this.

## `LGalleyStock`

One sheet the chosen destination offers, as the galley view lists it.
It carries facts only, so the gate chooses the wording it shows.
The name is the sheet's standard name, or null for an unnamed form.
The record also carries the upright width and height in inches and whether the setting uses it.

## `LGalleyRelay`

The engine's proof and print relay for one print window.
It draws the proof and its pages, prints, opens the system dialog, and ends the session.
The setting it draws and prints comes from its `LGalley`, which owns every setting edit.
The relay holds no board or proof.
`LGalleyClerk` holds both, and owns the latest-wins rule over draws.
Each relay member starts at most one draw, print or dialog in that clerk.
The board's browser is bound to the thread that opened it, so the relay never hops threads.
ShellEngine names no Core contract, so every board and proof call goes through the clerk.
The relay takes ownership of the clerk and disposes it.
The galley must read the same clerk, so its page count follows this relay's draws.
`LGalleyRelayGalley` hands the print gate the galley, so one opening hands out both halves.

## `LGalleyRelayDraw`

It draws a proof of the current setting through the clerk, and the latest request wins.
A proof that already matches the setting's layout is kept without a redraw.
After a draw that is not superseded, the galley parses its typed range again against the new page count.
It answers the proof's page count, first page size and default resolution.
The size is in device-independent pixels, 96 to the inch, which the canvas lays sheets out in.
It answers null when a newer draw superseded this one.
A proof with no pages answers a zero size.
The page overload gives one page of the held proof as an image at a resolution in dots per inch.
A null resolution leaves the choice to the clerk's default.
Pages count from zero.
It answers null before the first draw, and when a newer proof replaced the one it read.
A page outside the proof or a resolution below one throws.

## `LGalleyRelayPrint`

It prints to the chosen printer, or saves a PDF file when Save as PDF is chosen.
The relay owns that choice, so a gate makes this one call either way.
`path` is asked only for Save as PDF, so the gate asks the reader inside its own call.
It receives the galley's file name as the offered name, so the gate reads nothing else off the galley.
A null path cancels the save, which is no failure and answers done.
The gate tracks whether the answered task still runs, so the relay keeps no running flag.
`LGalleyRelayShow` opens the system print dialog on the sheet, for driver options the window does not offer.

## `DisposeAsync`

It disposes the session clerk, which ends any draw in flight and closes the board.
The board twin waits for its call in flight, so closing mid-draw never tears the browser down under it.
A second dispose does nothing.

## `LGalleyView`

The galley's ready answer for the print window's settings column, as facts with every value decided.
It names no wording key, since choosing wording is the gate's concern.
A null field means the control is hidden or disabled, except the margin name.
The destinations list Save as PDF first with a null name.
Copies and sheet count are null for Save as PDF, and collation is null when it is not offered.
The range text is null unless typed pages are chosen.
Valid is false only when that text names no valid pages.
One included flag per proof page is true when the page prints.
The margin name is the chosen preset, or null for a custom margin.
Then the four margins show in the region's unit.
Colour and sides are null for Save as PDF, and the default folds to Full and Single.
Known is false only when the chosen printer did not report its options.
Stale says the held proof no longer matches the layout.

## `LGalleyViewRead`

It decides everything the print window's settings column shows, so the gate only maps it.
It reads the galley's live state and keeps none, so the galley file holds only the session.
The chosen printer, offered sheets, page flags and sheet count come ready from `LGalley.LGalleyDigest`.
So the view names no Core rule and answers facts only.
The range text and its validity show only for the Custom scope.
Save as PDF hides copies, collation, colour, sides and the sheet count, since a PDF file has none.
Collation shows only for several copies on a printer that can collate.
The printer's own default colour and sides show as Full and Single.
The window offers no default choice of its own.
Each sheet carries a Core standard name or null, unchecked here.

## Stamp

Commit: `c2f335c331fe`
- `LGalley.cs` `170594a678c0cf8b`
- `LGalleyDestination.cs` `1fa0aa199dab0f0a`
- `LGalleyProof.cs` `ec058355dde52ee4`
- `LGalleyRelay.cs` `c5651eb3c3eab834`
- `LGalleyStock.cs` `11fcb031a5ccf774`
- `LGalleyView.cs` `d9c8401caf636bf0`
