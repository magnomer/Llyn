# TPressWindowChoice.cs
Hash: `e65a79eaf2bf0625`

## `public sealed class TPressWindowChoice`

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
