# QEnvoy.cs
Hash: `d1f3da6060d85254`

## `internal sealed class QEnvoy : CEnvoy`

The GUI's answer to Conduct's user-question port.
It puts up each box, dialog and window over `QEnvoyOwner`, worded from the key Conduct chose.

## `internal QEnvoy(Window surface, QPosture posture)`

Takes the main window, which owns every box while no other window of the app is active.
It also takes the window's posture, where the print window keeps its size.
The print setting itself is kept by the engine's store.

## `private Window QEnvoyOwner`

The one place that decides which window owns a box, a dialog or a file picker.
It is the active window of the app on the calling thread, else the main window.
A failure raised under the modal print window so sits above that window, not behind it.
A call off the app's thread, or with no app, falls back to the main window.

## `public bool CEnvoyConfirm(string key)`

Asks the question under a warning, and answers true on yes.

## `public bool CEnvoyConfirm(string key, string tallyKey, int tally)`

Asks the question with the tally of what it reaches on a line of its own.
It answers true on yes.
It is the question before a shared record is deleted, so it shows under a question mark.

## `public bool CEnvoyUnionConfirm(string key, string dropped, string kept)`

Asks before one record is folded into another, naming both so the direction of the fold is plain.
The dropped name comes first and the kept name second, joined by an arrow, because the fold reads that way.

## `public void CEnvoyFailureShow(string key, CLedgerNotice notice)`

Shows a failure under the localized headline `key` names, with the detail `notice` carries.
The notice's key is localized into the detail line.
A notice with a label and a path adds the localized label and the path on lines of their own.
It asks Conduct nothing further, so the box only places what the notice holds.

## `public void CEnvoyFailureShow(string key)`

Presents a request that failed with nothing further to say about why.
The headline alone is the whole answer, and an empty detail line under it would read as a missing message.

## `public bool? CEnvoyLeaveConfirm()`

Puts up the leave dialog, which offers three answers rather than two.
A leaving user often means to keep the word, so discarding is not the only exit.

## `public string? CEnvoyMarkupRead()`

Opens the markup file dialog over the owner window and answers the file picked.
A cancelled dialog answers null, which the gate reads as nothing to import.

## `public bool CEnvoyCustomsRead(CSCustoms customs)`

Puts up the customs window over the gate Conduct handed it and answers whether the user accepted.
The declared rows stay in the gate, so the window hands none back.

## `public bool CEnvoyPressRead(CSPress press)`

Opens the print window over the owner window and answers whether the reader printed or saved.
Every Print command reaches it through the print gate, so every command shows the preview.

## `public void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions)`

Puts up the customs window's report of what an import could not place.

## `public (string? CEnvoyFile, CPortraitMedium CEnvoyMedium) CEnvoyFileRead(string file, IReadOnlyList<CPortraitChoice> choices)`

Opens the save dialog with the formats the gate offers and the name it suggests.
The dialog lists the rows in order and starts on the chosen one.
The dialog's one-based filter index picks the medium from that same list.
So the filter and the format cannot drift apart.
A cancelled dialog answers no file, which the gate reads as nothing to export.
The dialog's title stays here, since only a windowed chooser has one.

## `public string? CEnvoyWorkspaceRead(string workspace)`

Opens the folder dialog on `workspace`, the folder in use, and answers the folder chosen.
A cancelled dialog answers null, which the gate reads as nothing to move.
The dialog's title stays here, since only a windowed chooser has one.

## `public string? CEnvoyCoinageRead(string key)`

Puts up the coinage dialog over the owner window, saying the message `key` names.
It answers the wording as typed, or null when the user retreats.
The mint button stays dark on a blank wording, so a blank one never comes back.
