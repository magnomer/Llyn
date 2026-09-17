# PWindowPress.cs

## `public partial class PWindow`

The window's part in printing: the printer choice and the words a page is written with.
What is printed is never read from the screen, so the panels ask the engine and hand it these.

## `internal async Task PWindowPressRun(Func<LPressTicket, Task> print)`

Asks the reader for a printer, then runs `print` with what was chosen.
A cancelled dialog prints nothing, and any failure is reported by the window.
The dialog stands inside the guard because it raises itself when the spooler is down or no printer exists.
Every panel prints through here, so the eight print buttons share one failure path.

## `internal LPressTicket? PWindowTicketRead()`

Opens the printer dialog and returns what the reader chose, or null when they cancelled.
Everything the dialog offers is carried: printer, sheet, turn, copies, collation, sides and color.
A choice the dialog left unsaid maps to the default the printer applies on its own.

## `private static LPressPaper PWindowPaperRead(PageMediaSize? size)`

The dialog measures the sheet in device-independent pixels, ninety-six to the inch.
A dialog that named no sheet gets the regional default rather than the browser's Letter.

## `internal LPortraitLabel PWindowLabelRead()`

The localized words an entry likeness is written with: the stand-in mark and every section heading.
Every panel that exports or prints an entry reads them here, so all agree.

## `internal LPortraitLegend PWindowLegendRead(string realm)`

The localized words a page likeness is written with, for the realm named: Example, Source or Situation.
The realm picks the untitled fallback and the usage tally wording, which differ by realm.
The source kinds are named in the reader's language, keyed by the enum the engine stores.
