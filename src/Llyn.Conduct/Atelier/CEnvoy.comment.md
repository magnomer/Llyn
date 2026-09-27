# CEnvoy.cs

## `public interface CEnvoy`

The port through which Conduct asks the user.
Each driver implements it in its own medium, so a gate never shows a dialog itself.
Conduct chooses the text key, and the driver looks up its wording.

## `bool CEnvoyConfirm(string key);`

Asks the yes-or-no question named by `key`, and answers true on yes.

## `void CEnvoyFailureShow(string key);`

Tells the user the failure named by `key`.

## `bool CEnvoyDiscardConfirm();`

Asks whether unsaved work may be stored or dropped, and answers true once it is settled either way.
A driver that holds editors of its own walks them, so the question keeps its own medium.
`CAtelierWorkspaceChange` asks it before leaving a workspace.

## `bool? CEnvoyLeaveConfirm();`

Asks whether unsaved work is stored, dropped, or kept open.
True stores, false discards, and null means the user stays.
