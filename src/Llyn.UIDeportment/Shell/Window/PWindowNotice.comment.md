# PWindowNotice.cs

## `public partial class PWindow`

What the window says to the user in its own voice.
It is the two message boxes the shell puts up.
Those are a request that failed, and the question asked before work is thrown away.

## Inline notes

### `internal void PWindowFailureShow(string key, Exception exception)`

Presents a request that failed, under the localized headline the given key names.
A deliberate refusal carries a reason key and resolves through the same catalog as the rest of the interface.

### `private string PWindowDetailRead(Exception exception)`

Reads the detail line out of a failure.

A refusal is a position the user can act on, so its reason is what they are shown.
The engine reads the reason through `CLedgerNoticeRead`, since the refusal type is its own.
Anything else is a fault.
A fault's own message is written for whoever fixes the program, not for whoever uses it.
A column name and an ordinal tell the user nothing they can act on.
The box says plainly that something unexpected went wrong.
The fault itself is written to the workspace's audit log and the box names the file.
So nothing diagnosable is lost, and nobody is handed a stack trace they did not ask for.

### `internal bool PWindowDiscardConfirm()`

Hands every editor the window closes over to `CAtelierQuitConfirm`, each with its two answers.
The two lists run in the same order, so a panel is asked and finished as one.
A panel whose edits have no store path yet belongs in neither list.
The leave question itself is put through `CEnvoy`.
