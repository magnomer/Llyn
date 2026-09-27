# PWindowPress.cs

## `public partial class PWindow`

The window's part in printing: the printer choice and the words a page is written with.
What is printed is never read from the screen, so the panels ask the engine and hand it these.

## `internal async Task PWindowPressRun(Func<CPressTicket, Task> print)`

Asks the reader for a printer, then runs `print` with what was chosen.
A cancelled dialog prints nothing, and any failure is reported by the window.
The dialog stands inside the guard because it raises itself when the spooler is down or no printer exists.
Every panel prints through here, so the eight print buttons share one failure path.

## `internal Task PWindowPressRun(Func<CPortraitLabel, CPressTicket, Task> print)`

Prints through a sealed panel that takes the window's labels with the ticket.

## `internal Task PWindowPressRun(string realm, Func<CPortraitLabel, CPortraitLegend, CPressTicket, Task> print)`

Prints through a sealed panel that takes the labels and the realm's legend with the ticket.
The window reads the legend, so the panel hands over no word of its own.

## `internal Task PWindowPressRun(string realm, Func<CPortraitLegend, CPressTicket, Task> print)`

Runs a legend print for a sealed panel that prints no label, such as the guild.
The legend is worded for `realm` once the ticket is chosen.

## `internal Task PWindowPressRun(Func<LPortraitLabel, LPressTicket, Task> print)`

Prints through a panel not yet sealed that takes the labels too, wrapped the same way.
It goes once every panel takes Conduct shapes.

## `internal CPressTicket? PWindowTicketRead()`

Opens the printer dialog and returns what the reader chose, or null when they cancelled.
Everything the dialog offers is carried: printer, sheet, turn, copies, collation, sides and color.
A choice the dialog left unsaid maps to the default the printer applies on its own.

## `private static double? PWindowPaperRead(double? pixels)`

The dialog measures the sheet in device-independent pixels, ninety-six to the inch.
A side the dialog left unnamed or empty is none, and the panel then takes the regional default.

## `internal CPortraitLabel PWindowPortraitRead()`

The localized words an entry likeness is written with: the stand-in mark and every section heading.
Every panel that exports or prints an entry reads them here, so all agree.

## `internal CPortraitLegend PWindowLegendCreate(string realm)`

The localized words a page likeness is written with, for the realm named: Example, Source or Situation.
The realm picks the untitled fallback and the usage tally wording, which differ by realm.
The source kinds are named in the reader's language, keyed by each kind's localization key.
