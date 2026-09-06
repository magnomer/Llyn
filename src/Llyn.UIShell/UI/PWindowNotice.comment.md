# PWindowNotice.cs

## `public partial class PWindow`

What the window says to the user in its own voice.
That is the localized text lookup every panel reads its wording through.
It is also the two message boxes the shell puts up.
Those are a request that failed, and the question asked before work is thrown away.

## Inline notes

### `internal void PWindowFailureShow(string key)`

Presents a request that failed with nothing further to say about why.
A disagreement the program noticed itself carries no refusal and no fault.
The headline alone is the whole answer, and an empty detail line under it would read as a missing message.

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

### `private (Func<bool> Check, Func<bool, bool> Finish)[] PWindowEditorRead()`

The editors the window closes over, each paired with its own two answers.
Asking and finishing read the same list, so a panel cannot be asked about work the exit would not carry.
A panel whose edits have no store path yet belongs in neither, because a prompt that promises a save is worse than no prompt.
The corpus panel is on the list now that its sentence editor holds a draft it can commit.
The repertoire panel joins it on the same terms, its situation editor holding one too.
The reference panel completes the list, so every editor in the shell is both asked and finished.

### `internal bool PWindowDiscardConfirm()`

Asks before typed text is thrown away, and answers whether it may be.
Every editor on the list is asked whether it holds unsaved work.
Each is asked even after one has answered yes, because the asking is what writes a pause-held keystroke down.
The window closing and the workspace changing take all of them with it.
Unsaved work now offers three answers rather than two, because discarding was the only exit and a leaving user often means to keep the word.
Saving commits every draft the window holds, discarding cancels every one of them, and the third answer leaves the window standing.
Either answer that lets the window go empties the drafts folder of this window's files.
A leftover would otherwise be reported at the next launch as work a crash cost, which a clean exit never did.
A panel holding nothing unsaved is settled without a question, since its blank draft has nothing to lose either way.
Saving that the engine refuses answers no, so the window stays open over the entry it failed to store.

### `private bool PWindowDraftFinish((Func<bool> Check, Func<bool, bool> Finish)[] editors, bool store)`

Carries one answer to every editor on the list the question was put over.
Each holds its own draft under its own origin, so each is told separately.
An unchanged draft is cancelled whatever the answer, because storing what matches its entry would rewrite the entry for nothing.
Every editor is told before the answers are read, so one refusal does not leave the others still holding their drafts.
The window may go only when all of them are finished.

### `internal bool PWindowDiscardConfirm(bool unsaved)`

The same question over one panel's own answer.
It is for a panel leaving its editing state while the rest of the window stays as it is.
Nothing unsaved means nothing to ask about.
So the question is only ever put when there is something to lose.
That is why it can sit in front of every path that discards typed work.

### `internal bool PWindowRemovalConfirm(int usage, string scope)`

Asks before a shared record is deleted, and says how much the delete reaches.
A record nothing references is a plain question.
One something references names the number of places first, because the delete drops those references too.
`scope` names the kind of record, so a Situation and an Example each speak of themselves.
The wording is the panel's, but the shape of the question belongs to the window that asks it.
