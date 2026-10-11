# LGalleyRelay.cs
Hash: `c5651eb3c3eab834`

## `public sealed class LGalleyRelay : IAsyncDisposable`

The engine's proof and print relay for one print window.
It draws the proof and its pages, prints, opens the system dialog, and ends the session.
The setting it draws and prints comes from its `LGalley`, which owns every setting edit.
The relay holds no board or proof.
`LGalleyClerk` holds both, and owns the latest-wins rule over draws.
Each relay member starts at most one draw, print or dialog in that clerk.
The board's browser is bound to the thread that opened it, so the relay never hops threads.

## `private readonly LGalleyClerk _lGalleyRelayClerk`

The session clerk that holds the board and the proof of the last draw.
ShellEngine names no Core contract, so every board and proof call goes through it.

## `internal LGalleyRelay(LGalleyClerk clerk, LGalley galley)`

Takes ownership of `clerk`, which the relay disposes.
`galley` must read the same clerk, so its page count follows this relay's draws.

## `public LGalley LGalleyRelayGalley`

The setting session this relay draws and prints.
The print gate takes it from here, so one opening hands out both halves.

## `public async Task<LGalleyProof?> LGalleyRelayDraw()`

Draws a proof of the current setting through the clerk, and the latest request wins.
A proof that already matches the setting's layout is kept without a redraw.
After a draw that is not superseded, the galley parses its typed range again against the new page count.
It answers the proof's page count, first page size and default resolution.
The size is in device-independent pixels, 96 to the inch, which the canvas lays sheets out in.
It answers null when a newer draw superseded this one.
A proof with no pages answers a zero size.

## `public Task<LPressImage?> LGalleyRelayDraw(int page, int? resolution)`

One page of the held proof as an image at `resolution` dots per inch.
A null resolution leaves the choice to the clerk's default.
Pages count from zero.
It answers null before the first draw, and when a newer proof replaced the one it read.
A page outside the proof or a resolution below one throws.

## `public async Task<LPressStatus> LGalleyRelayPrint(Func<string, string?> path)`

Prints to the chosen printer, or saves a PDF file when Save as PDF is chosen.
The relay owns that choice, so a gate makes this one call either way.
`path` is asked only for Save as PDF, so the gate asks the reader inside its own call.
It receives the galley's file name as the offered name, so the gate reads nothing else off the galley.
A null path cancels the save, which is no failure and answers done.
The gate tracks whether the answered task still runs, so the relay keeps no running flag.

## `public Task LGalleyRelayShow()`

Opens the system print dialog on the sheet, for driver options the window does not offer.

## `public ValueTask DisposeAsync()`

Disposes the session clerk, which ends any draw in flight and closes the board.
The board twin waits for its call in flight, so closing mid-draw never tears the browser down under it.
A second dispose does nothing.
