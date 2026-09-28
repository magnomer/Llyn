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
The host walks its editors and shows the store, discard or stay question, so the GUI keeps its own dialog.
It answers true only once every editor has finished.

## `public bool? CEnvoyLeaveConfirm()`

Puts up the leave dialog, which offers three answers rather than two.
A leaving user often means to keep the word, so discarding is not the only exit.
