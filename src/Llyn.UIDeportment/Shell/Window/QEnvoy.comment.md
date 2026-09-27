# QEnvoy.cs

## `internal sealed class QEnvoy : CEnvoy`

The GUI's answer to Conduct's user-question port.
It puts up a message box over the main window, worded from the key Conduct chose.

## `public bool CEnvoyConfirm(string key)`

Asks the question under a warning, and answers true on yes.

## `public void CEnvoyFailureShow(string key)`

Presents a request that failed with nothing further to say about why.
The headline alone is the whole answer, and an empty detail line under it would read as a missing message.
