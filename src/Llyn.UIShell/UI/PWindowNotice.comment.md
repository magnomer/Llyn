# PWindowNotice.cs

## `public partial class PWindow`

What the window says to the user in its own voice: the localized text lookup every panel reads its wording through, and the two message boxes the shell puts up — a request that failed, and the question asked before typed work is thrown away.

## Inline notes

### `internal void PWindowFailureShow(string key, Exception exception)`

Presents a request that failed, under the localized headline the given key names. A deliberate refusal carries a reason key, which resolves through the same catalog as the rest of the interface; anything unexpected is a fault rather than a refusal, so it keeps its own message, which stays diagnosable even though it is not translated.

### `private string PWindowDetailRead(Exception exception)`

Reads the detail line out of a failure, following it down to what actually went wrong.

A failure that wraps another says where the work stopped and not why it stopped: an import that names the entry it broke on carries the broken tag inside it. So each wrapper's own message is kept and the one it wraps is read after it, down to the innermost, and a refusal at any depth resolves through the catalog like one at the top. Showing only the outermost message would leave the user holding a position with no reason attached, which is the shape of a failure nobody can act on.

### `internal bool PWindowDiscardConfirm()`

Asks before typed text is thrown away, and answers whether it may be. Every panel that edits an entry is asked whether it holds unsaved work, because the window closing and the workspace changing take all of them with it.

### `internal bool PWindowDiscardConfirm(bool unsaved)`

The same question over one panel's own answer, for a panel that is leaving its editing state while the rest of the window stays as it is. Nothing unsaved means nothing to ask about, so the question is only ever put when there is something to lose — which is why it can sit in front of every path that discards typed work.
