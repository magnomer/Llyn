# CEnvoy.cs

## `public interface CEnvoy`

The port through which Conduct asks the user.
Each driver implements it in its own medium, so a gate never shows a dialog itself.
Conduct chooses the text key, and the driver looks up its wording.

## `bool CEnvoyConfirm(string key);`

Asks the yes-or-no question named by `key`, and answers true on yes.

## `void CEnvoyFailureShow(string key);`

Tells the user the failure named by `key`.
