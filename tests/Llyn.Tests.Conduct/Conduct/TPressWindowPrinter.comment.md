# TPressWindowPrinter.cs
Hash: `f9badfbe67cfb7c2`

## `public sealed class TPressWindowPrinter`

Covers the printer choice and the fields it governs.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.

A fresh setting lists Save as PDF first and the default printer chosen.
A spooler that fails still opens the window, on Save as PDF alone.
The failure reaches the ledger as `Print.Failed`.
Save as PDF hides copies, collation, colour, sides, sheets and the hint.
Collation and the hint are each shown with a value before Save as PDF is chosen.
A printer is chosen by the picker's index into the listed printers.
An index of -1 or past the list keeps the chosen printer.

Copies clamp to the printer's maximum and to one, and an unreported maximum clamps to 999.
Scale clamps to 10 and 200.
A text that is no number keeps the old copies and scale.
Collation shows only with several copies, and a choice made there is stored.

A printer that reported nothing falls back to the standard papers and shows the capability hint.
It also hides collation, even with several copies.
Its colour and sides fall back to full and single.
A printer that reported them keeps gray and long edge, and an unreported short edge falls back.

Typed margins store a custom margin, and typed zeros pick the None preset.
A margin text that is no number keeps the old margin.

The margin and two-sided facts read the chosen row of the setting's choice lists.
