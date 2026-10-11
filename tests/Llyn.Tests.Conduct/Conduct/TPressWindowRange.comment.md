# TPressWindowRange.cs
Hash: `ba7d94550c422f51`

## `public sealed class TPressWindowRange`

Covers which pages print and how many sheets they take.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, the real print route over a fake board.
It is one sealed class per concern of the print window, with no shared part.

Before a draw the setting lists no pages and is stale.
A draw includes every page.
A range with an open start or end includes the pages it names.
A page outside the proof, a reversed span or text with no number reports `Print.PagesInvalid`.
The typed text stays, and every page stays included.
The Current range follows the selected page, and a typed page number clamps to the proof.
A page text that is no number keeps the old page.
Text typed alone keeps the chosen mode and raises the setting once.

Sheets halve for two-sided printing and multiply by copies.
Exactly one sheet picks the singular wording key.
