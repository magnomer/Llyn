# TPressWindowMemory.cs
Hash: `bb3ffcc626e253a5`

## `public sealed class TPressWindowMemory`

Covers the setting the window keeps through the store and restores in the next window.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.

A setting the first window saved on closing restores over a proof drawn with another layout.
It drops the typed range and makes the proof stale.
An empty store keeps the setting.
Unreadable text is the store's own concern, checked in `TPressFile`.
The restore policy for unlisted printers is checked in `TPressClerk`.
The restored pages mode is read as the chosen row of the setting's range list.
