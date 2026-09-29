# CEnvoy.cs

## `public interface CEnvoy`

The port through which Conduct asks the user.
Each driver implements it in its own medium, so a gate never shows a dialog itself.
Conduct chooses the text key, and the driver looks up its wording.

## `bool CEnvoyConfirm(string key);`

Asks the yes-or-no question named by `key`, and answers true on yes.

## `bool CEnvoyConfirm(string key, string tallyKey, int tally);`

Asks the question named by `key` with a tally under it, and answers true on yes.
`tallyKey` names the label of the count, so the driver words both from its own catalog.

## `bool CEnvoyUnionConfirm(string key, string dropped, string kept);`

Asks the question named by `key` before one record is folded into another, and answers true on yes.
`dropped` and `kept` are the two names, which the driver shows as given in the fold's direction.

## `void CEnvoyFailureShow(string key);`

Tells the user the failure named by `key`.

## `void CEnvoyFailureShow(string key, CLedgerNotice notice);`

Tells the user the failure named by `key`, with the ready detail Conduct read for it.
Conduct read the notice once and recorded any fault, so the driver only words the keys it carries.

## `bool? CEnvoyLeaveConfirm();`

Asks whether unsaved work is stored, dropped, or kept open.
True stores, false discards, and null means the user stays.

## `IReadOnlyList<CSCustomsRow>? CEnvoyCustomsRead(IReadOnlyList<CMarkupEntry> entries);`

Asks how each parsed entry of a markup file enters the workspace, one declared row per entry in file order.
It answers null when the user declines, and the gate then stores nothing.

## `void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions);`

Tells the user what a markup import could not place, line by line.

## `(string? CEnvoyFile, CPortraitMedium CEnvoyMedium) CEnvoyFileRead(string file, IReadOnlyList<CPortraitChoice> choices);`

Asks where an export is written and in which of `choices`, offering `file` as the name.
It answers no file when the user declines, and the gate then exports nothing.
The gate chooses the offered name and the formats, so the question needs no read of its own.

## `CPressTicket? CEnvoyTicketRead();`

Asks which printer a print goes to and how, and answers the dialog's choice as a ticket.
It answers null when the user declines, and the gate then prints nothing.
The gate asks it inside its failure policy, so a dialog that fails is reported as `Print.Failed`.
