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

## `void CEnvoyFailureShow(string key, Exception exception);`

Tells the user the failure named by `key`, with the exception the engine threw.
The driver words the detail and records a fault, so nothing diagnosable is lost.

## `bool CEnvoyDiscardConfirm();`

Asks whether unsaved work may be stored or dropped, and answers true once it is settled either way.
A driver that holds editors of its own walks them, so the question keeps its own medium.
`CAtelierWorkspaceChange` asks it before leaving a workspace.

## `bool? CEnvoyLeaveConfirm();`

Asks whether unsaved work is stored, dropped, or kept open.
True stores, false discards, and null means the user stays.
