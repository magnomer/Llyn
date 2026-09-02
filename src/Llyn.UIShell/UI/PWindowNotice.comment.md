# PWindowNotice.cs

## `public partial class PWindow`

What the window says to the user in its own voice.
That is the localized text lookup every panel reads its wording through.
It is also the two message boxes the shell puts up.
Those are a request that failed, and the question asked before work is thrown away.

## Inline notes

### `internal void PWindowFailureShow(string key, Exception exception)`

Presents a request that failed, under the localized headline the given key names.
A deliberate refusal carries a reason key.
It resolves through the same catalog as the rest of the interface.
Anything unexpected is a fault rather than a refusal, so it keeps its own message.
That message stays diagnosable even though it is not translated.

### `private string PWindowDetailRead(Exception exception)`

Reads the detail line out of a failure, following it down to what actually went wrong.

A failure that wraps another says where the work stopped, not why it stopped.
An import that names the entry it broke on carries the broken tag inside it.
So each wrapper's own message is kept and the one it wraps is read after it.
That goes down to the innermost.
A refusal at any depth resolves through the catalog like one at the top.
Showing only the outermost message would leave the user holding a position with no reason.
That is the shape of a failure nobody can act on.

### `internal bool PWindowDiscardConfirm()`

Asks before typed text is thrown away, and answers whether it may be.
Every panel that edits an entry is asked whether it holds unsaved work.
The window closing and the workspace changing take all of them with it.

### `internal bool PWindowDiscardConfirm(bool unsaved)`

The same question over one panel's own answer.
It is for a panel leaving its editing state while the rest of the window stays as it is.
Nothing unsaved means nothing to ask about.
So the question is only ever put when there is something to lose.
That is why it can sit in front of every path that discards typed work.

### `internal bool PWindowRemovalConfirm(int usage)`

Asks before a shared record is deleted, and says how much the delete reaches.
A record nothing references is a plain question.
One something references names the number of places first, because the delete drops those references too.
