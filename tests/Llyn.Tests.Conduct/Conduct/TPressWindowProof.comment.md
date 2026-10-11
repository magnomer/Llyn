# TPressWindowProof.cs
Hash: `d8b5763f3a09e514`

## `public sealed class TPressWindowProof`

Covers the proof the window draws from the setting.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.

Only orientation, paper, margin, scale and background make the proof stale, and a draw clears it.
A first draw reports the chosen paper at 96 pixels per inch.
An unchanged setting skips the second render, and a changed one renders the latest setting.
A draw held open while a newer setting arrives answers null, and the newer draw renders the latest setting.
A landscape draw hands the orientation to the board and reports the turned page size.

The page draw has no gate of its own, since only the sight pass draws pages.
Its PNG and page index are held by `TPressWindowDelay`, and its resolution and range by `TGalleyClerk`.
A board whose draw fails returns the failure state for the retry veil and shows the same key.
The press stays stale, but the failure state's setting does not, so the window shows no busy veil with it.
