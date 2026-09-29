# QEnvoy.cs

## `internal sealed class QEnvoy : CEnvoy`

The GUI's answer to Conduct's user-question port.
It puts up a message box over the main window, worded from the key Conduct chose.

## `internal QEnvoy(Window surface, PWindow host)`

Takes the loaded window that owns every box, and the host whose editors the discard question walks.

## `public bool CEnvoyConfirm(string key)`

Asks the question under a warning, and answers true on yes.

## `public bool CEnvoyConfirm(string key, string tallyKey, int tally)`

Asks the question with the tally of what it reaches on a line of its own.
It answers true on yes.
It is the question before a shared record is deleted, so it shows under a question mark.

## `public bool CEnvoyUnionConfirm(string key, string dropped, string kept)`

Asks before one record is folded into another, naming both so the direction of the fold is plain.
The dropped name comes first and the kept name second, joined by an arrow, because the fold reads that way.

## `public void CEnvoyFailureShow(string key, Exception exception)`

Presents a failure that carries an exception through the window, which words the detail and records a fault.

## `public void CEnvoyFailureShow(string key)`

Presents a request that failed with nothing further to say about why.
The headline alone is the whole answer, and an empty detail line under it would read as a missing message.

## `public bool CEnvoyDiscardConfirm()`

Asks whether unsaved work may be stored or dropped before a workspace change.
It hands itself to `CAtelierQuitConfirm`, which asks every area and shows the one question through this envoy.
It answers true only once every area has finished.

## `public bool? CEnvoyLeaveConfirm()`

Puts up the leave dialog, which offers three answers rather than two.
A leaving user often means to keep the word, so discarding is not the only exit.

## `public IReadOnlyList<CSCustomsRow>? CEnvoyCustomsRead(IReadOnlyList<CMarkupEntry> entries)`

Puts up the customs window over the parsed entries and answers the rows the user declared.
A cancelled window answers null.

## `public void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions)`

Puts up the customs window's report of what an import could not place.

## `public (string? CEnvoyFile, CPortraitMedium CEnvoyMedium) CEnvoyFileRead(string file, IReadOnlyList<CPortraitChoice> choices)`

Opens the save dialog with the formats the gate offers and the name it suggests.
The dialog lists the rows in order and starts on the chosen one.
It returns a one-based index into that same list, so the filter and the format cannot drift apart.
A cancelled dialog answers no file, which the gate reads as nothing to export.
The dialog's title stays here, since only a windowed chooser has one.

## `public CPressTicket? CEnvoyTicketRead()`

Opens the printer dialog and answers what the reader chose, or null when they cancelled.
Everything the dialog offers is carried: printer, sheet, turn, copies, collation, sides and color.
The sheet is carried as the dialog measured it, and the engine turns it into inches.
A choice the dialog left unsaid maps to the default the printer applies on its own.
A dialog that raises, as with the spooler down, is reported by the gate as `Print.Failed`.
