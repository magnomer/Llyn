# Press

The Press folder holds the print session rules and the session's hold on the board.
ShellEngine may not name the Core board, proof or setting rules, so it reaches them through these clerks.

## `LGalleyClerk`

The print session's hold on the board and on the proof it drew last.
`LPortraitClerkPress.LPortraitClerkOpen` builds one around a freshly opened board.
The clerk is the single owner of the latest-wins rule.
Every proof draw takes a generation, and a draw that returns after a newer one started drops its own proof.
The proof and the setting it was drawn from change together, so the held proof always matches its setting.
There is no turn chain.
The board twin already serialises every board call, and the generation orders the results.
So the clerk relies on a board that runs one call at a time.
There is no `ConfigureAwait(false)` here.
The board is bound to the thread that opened it.
The clerk takes ownership of the board and disposes it.

### `LGalleyClerkResolution`

The resolution in dots per inch a page is drawn at when the caller names none.
The clerk owns it, so Conduct's lens asks for more detail only when the zoom needs it.
300 is print quality, sharp on screen up to about three times actual size.

### `_lGalleyClerkReader`

The page draws in flight, counted per proof.
A replaced proof with readers stays open until its last reader ends, and that reader disposes it.
It also serves as the lock over the held proof and the counts.

### `LGalleyClerkCount`

The page count of the held proof, or null before a draw and after dispose.

### `LGalleyClerkCheck`

Whether the held proof still shows the setting.
Only the layout counts, through `LPressSetting.LPressSettingMatch`.
Nothing held never matches, so the session draws.

### `LGalleyClerkDraw`

The setting overload draws a proof and holds it, answering false when a newer draw superseded this one.
The generation is taken before the up-to-date check.
So a draw that finds the held proof current still supersedes any older draw in flight.
A superseded proof is disposed here, and the held one is never touched by it.
A draw after dispose throws `ObjectDisposedException`.
The page overload draws one page of the held proof as a PNG.
A null resolution draws at the default resolution.
It answers null when no proof is held, or when a newer proof replaced this one during the draw.
That is latest-wins, not a failure, so it never throws for it.
A page outside the proof, or a resolution below one, still throws.
The proof is counted as read for the whole draw.
A draw or dispose that replaces it then leaves the disposing to this reader.

### `LGalleyClerkRead`

The size of one page of the held proof, in inches.
It answers null when no proof is held or the proof has no such page.

### `LGalleyClerkPrint`

Prints the setting on its printer, or exports it as PDF to a file when one is given.
An export answers done, since only printing reports a status.

### `LGalleyClerkShow`

Opens the system print dialog over the board.

### `DisposeAsync`

Ends every draw in flight by taking a new generation, so each disposes its own proof when it returns.
It drops the held proof unless a page draw still reads it, then disposes the board.
The board twin waits for its call in flight before it closes.

### `LGalleyClerkSet`

Replaces the held proof and its setting as one step.
The replaced proof is disposed at once when no page draw reads it.

## `LPressClerk`

The print setting rules a print session runs, so ShellEngine names no Core rule.
It holds no session state, only the store port taken from the rig.
Apart from restore and save, every member takes its inputs as data and answers a new value.
The Core records keep the facts intrinsic to them, and this clerk is their only caller in the engine.
`LWorkspaceStaff` builds one per rig, so a workspace change swaps the store with the rest.

### `LPressClerkCreate`

The opening setting for the default printer, else the first listed, else Save as PDF.
An empty list therefore answers the Save as PDF setting a session starts with.

### `LPressClerkSelect`

The setting moved to the named printer and clamped against it.
A null name means Save as PDF.
A name the list lacks throws, since the window offers only listed printers.

### `LPressClerkClamp`

The setting made valid for the printer it names, found in the list.
A name the list lacks clamps as Save as PDF would.

### `LPressClerkRestore`

The kept setting fitted to the listed printers, or null when the store holds none.
A kept printer stays only while listed, else the default or first listed printer stands in.
A kept null printer stays Save as PDF.
The page range comes back empty, since pages belong to one document.
A store that cannot be read raises `LVaultFault`, which the caller shows.

### `LPressClerkSave`

Keeps the setting for the next print window, with its page range emptied.
A store that cannot be written raises `LVaultFault`.

### `LPressClerkParse`

The range the typed text names against the page count, or null when the text is invalid.

### `LPressClerkResolve`

Typed margin values in the reader's unit give a preset when they equal one, else custom.
The preset overload gives the named preset, or the current values renamed custom when the name is null.
A name no preset carries throws, since the window offers only presets.

### `LPressClerkRead`

The ready facts for the window, given the drawn page count or null before a draw.
Two-sided printing halves the sheet count, rounding up, before copies multiply it.

### `LPressClerkFind`

The named overload answers the listed printer of that name, or null for Save as PDF or an unlisted name.
The other overload answers the default printer, else the first listed, else null when none is installed.

## `LPressDigest`

The ready facts a print session shows, each derived from the setting, the printer list and the page count.
`LPressClerk.LPressClerkRead` answers it, so the session reads facts and computes none.
It is answered fresh on each read and never kept, since every edit changes it.
`LPressDigestPrinter` is the listed printer the setting names, or null to save as PDF.
`LPressDigestPaper` is the sheets that printer offers.
  The standard sheets stand in when saving as PDF, or when the printer reported no sheet.
`LPressDigestChosen` is the index of the setting's sheet among them, or null when none matches.
`LPressDigestIncluded` holds one flag per drawn page, true when the range includes it, empty before a draw.
`LPressDigestSheets` is the sheets the job uses, halved for two-sided printing and multiplied by copies.

## Stamp

Commit: `c2f335c331fe`
- `LGalleyClerk.cs` `a7d1b05795d66092`
- `LPressClerk.cs` `2bfee7300a83c3a3`
- `LPressDigest.cs` `609fddabcf38bad6`
