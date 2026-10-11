# TPressWindowOutput.cs
Hash: `ed6d2f9405989c4e`

## `public sealed class TPressWindowOutput`

Covers the print, the export and the busy state they report.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.

A print hands the setting to the board and asks nothing.
An offline or failing printer returns its wording key.
Save as PDF asks for a file and exports to it, and a declined file exports nothing.
The system dialog is shown through the board.
A held print reports busy through the setting event, then idle once it ends.
A second print while one is held is refused with `Print.Printing`.
The window may close when idle, and the close saves the setting once.
A close while a print is held is refused and saves nothing.
A store that cannot be written shows `Print.Failed`, and the window still closes.
