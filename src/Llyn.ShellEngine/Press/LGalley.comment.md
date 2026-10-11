# LGalley.cs
Hash: `170594a678c0cf8b`

## `public sealed class LGalley`

The engine's setting session for one print window, from the window opening to its close.
It keeps the printers read when the window shows, the setting the reader is shaping, its scope and typed range.
Every option edit is clamped through `LPressClerk` against the chosen printer, so Conduct mirrors no print data.
It keeps no window state, so the page the canvas shows and the running flag live in the gate.
The galley names no Core rule, and holds Core records only as data.
A gate holds the galley as a panel holds a tenure, and reads ready answers from it.
The galley draws, prints and disposes nothing.
`LGalleyRelay` does that, and holds this galley as the setting it draws and prints.
The galley reads the proof's page count from the session clerk, never its board or proof.
The board's browser is bound to the thread that opened it.
So the galley is used from that one thread, and nothing in it hops threads.

## `private readonly LGalleyClerk _lGalleyClerk`

The session clerk the galley reads the proof's page count and freshness from.
`LGalleyRelay` owns this clerk and disposes it, so the galley only reads it.

## `private readonly LPortraitClerkPress _lGalleyPortrait`

The press clerk the galley lists printers through.
ShellEngine names no Core port, so the listing goes through Application.

## `private readonly LPressClerk _lGalleyPress`

The print setting clerk that owns every setting rule and the kept setting.
The galley keeps the session state, the clerk the rules over it.

## `internal LGalley(LGalleyClerk clerk, LPortraitClerkPress portrait, LPressClerk press, string file)`

Reads `clerk` without owning it, since the relay built beside it disposes it.
The session opens with no printers, on Save as PDF.
`LGalleyRestore` lists the printers when the window shows.
`file` is the name a PDF save is offered under.

## `public LPressSetting LGalleySetting`

The current setting, always clamped for the chosen printer.
Each edit replaces it whole, so a draw in flight keeps the setting it started with.

## `public IReadOnlyList<LPressPrinter> LGalleyPrinter`

The printers the last restore listed, in the order the spooler listed them.
It stays empty when listing failed, so only Save as PDF is offered.

## `public string LGalleyRange`

The page range text as the reader last typed it, kept even when invalid.
A new proof can change the page count, so the text is parsed again after each draw.

## `public LPressScope LGalleyScope`

The pages mode the reader chose, which decides the range text.
It opens on All, matching the empty range text.

## `public string LGalleyFile`

The file name a PDF save is offered under, formatted by the settings facade.

## `public bool LGalleyStale`

Whether the held proof no longer shows the setting's layout, or no proof exists yet.
The clerk holds the setting the proof was drawn with, so it answers this.
Copies, range, colour and sides never make it stale.
A printer change makes it stale only when the clamp changes the layout.

## `internal int? LGalleyCount`

The page count of the held proof, or null before the first draw.
The clerk owns the proof, so the galley reads the count and never stores it.

## `public bool LGalleyMetric`

Whether custom margins are typed in millimetres, read from the region once at open.

## `public bool LGalleyRangeValid`

Whether the typed range text names pages the held proof has.
Before the first draw only the form is checked, as in `LGalleyRangeSet`.

## `internal LPressDigest LGalleyDigest`

The ready facts the view shows, read fresh from `LPressClerk.LPressClerkRead`.
It passes the drawn page count, or null before the first draw.
`LGalleyPaperSelect` indexes into its offered sheets.

## `public LPressMargin LGalleyMargin`

The setting's margin with its values in the region's unit, which `LGalleyMetric` names.
It is the reverse of the four-value `LGalleyMarginSet`, so typed values round-trip.
Its name still marks a preset, so only the numbers change.

## `public void LGalleySave()`

Keeps the live setting in the store through `LPressClerk.LPressClerkSave`.
A store that cannot be written raises, and the gate reports it.

## `public async Task<LGalleyView> LGalleyRestore()`

Lists the printers, applies the kept setting to the live one and answers the ready view.
Restoring needs the printers, since a remembered printer is matched by name.
The kept setting is read and applied in a `finally`, so a listing failure still restores against no printers.
The failure then travels on to the gate, which reports it.
No `ConfigureAwait(false)` is used, so the setting changes on the window's thread.

## `private void LGalleyRestore(LPressSetting? saved)`

Applies the setting `LPressClerk.LPressClerkRestore` fitted to the listed printers.
Null means nothing is kept, which leaves an edited setting untouched.
A setting nobody edited yet takes the clerk's opening setting for the listed printers.
A kept setting resets the scope to all pages and clears the typed range.
The next draw renders when the restored layout differs from the held proof.

## `public LPressSetting LGalleyPrinterSelect(string? name)`

Chooses the printer by queue name, or Save as PDF when `name` is null.
`LPressClerk.LPressClerkSelect` clamps against the new printer, so options it lacks fall back.
A name outside `LGalleyPrinter` is a caller mistake and throws.

## `public LPressSetting LGalleyCopiesSet(string text)`

Sets the copy count the reader typed, clamped to the printer's limit.
Text that is not a whole number leaves the setting untouched, so a half-typed field moves nothing.

## `public LPressSetting LGalleyCollationSet(bool collation)`

Sets collation, which stays off for a printer that cannot collate.

## `public bool LGalleyRangeSet(LPressScope scope, string text, int page)`

Chooses the pages mode and sets the range text it stands for.
All sets empty text, Current sets `page`, and Custom sets `text`.
`page` is the canvas's page counted from zero, which the gate holds.
It answers whether the resulting text names pages that exist.

## `public LPressSetting LGalleyPageSelect(int page)`

Applies a new canvas page, counted from zero, to the range.
The gate keeps the page, so the galley only rewrites a Current range to it.
Paging never moves another scope's range.
A negative page is a caller mistake and throws.

## `public int LGalleyPageSelect(string text, int page)`

Answers the page whose number the reader typed, counted from one as a reader counts.
`page` is the gate's current page, answered back when nothing changes.
A number past either end selects the nearest page, so a typed page always lands somewhere.
Text that is no whole number, or no drawn proof with pages, leaves the page untouched.
A changed page goes through `LGalleyPageSelect(int)`, so a Current range follows it.

## `private bool LGalleyRangeSet(string text)`

Keeps `text` and answers whether it names pages that exist.
Before the first draw the page count is unknown, so only the form is checked.
An invalid text leaves the setting's last valid range in place.

## `private bool LGalleyRangeSet(int page)`

Sets the range to the one page `page`, counted from zero like the proof.
Range text counts from one, so the page is written one higher.

## `public LPressSetting LGalleyOrientationSet(LPressOrientation orientation)`

Sets the orientation, which makes the proof stale.

## `public LPressSetting LGalleyPaperSelect(int index)`

Chooses a sheet by its place in the offered sheets of `LGalleyDigest`.
An index outside that list is a caller mistake and throws.

## `public LPressSetting LGalleyMarginSet(LPressMargin margin)`

Sets a preset margin, or any margin already in inches.
A margin that leaves no room on the sheet falls back to the default.

## `public LPressSetting LGalleyMarginSet(string? name)`

Chooses a preset by its name through `LPressClerk.LPressClerkResolve`.
A null name keeps the current values as a custom margin, so choosing Custom moves nothing.
A name outside the presets is a caller mistake and throws.

## `public LPressSetting LGalleyMarginSet(string top, string bottom, string left, string right)`

Sets custom margins typed in the region's unit, which `LGalleyMetric` names.
The text is read in the current culture, the same culture the fields are shown in.
If any of the four is not a number, the setting stays untouched.
Values that equal a preset resolve to that preset.

## `public LPressSetting LGalleyScaleSet(string text)`

Sets the scale percent the reader typed, clamped to the range the browser accepts.
Text that is not a whole number leaves the setting untouched.

## `public LPressSetting LGalleyColorSet(LPressColor color)`

Sets the colour mode, which falls back to the printer default when the printer lacks it.

## `public LPressSetting LGalleySideSet(LPressSide side)`

Sets two-sided printing, which falls back to the printer default when the printer lacks it.

## `public LPressSetting LGalleyBackgroundSet(bool background)`

Sets whether background colours and pictures print.

## `internal void LGalleyRangeSet()`

Parses the typed range again against the page count of the proof just drawn.
`LGalleyRelay` calls it after each draw that a newer one did not supersede.
A range that does not parse leaves the setting's range as it was.
