# PWindowNotice.cs

## `public partial class PWindow`

What the window says to the user in its own voice.
That is the localized text lookup every panel reads its wording through.
It is also the two message boxes the shell puts up.
Those are a request that failed, and the question asked before work is thrown away.

## Inline notes

### `internal string? PLocalizationTextFind(string key)`

The localized text under `key`, or null when no locale declares it.
The plain read answers the key itself on a miss, which a caller cannot tell from a translation.
A variety name is shown raw when its key is missing, so the miss has to be visible.

### `internal void PWindowFailureShow(string key)`

Presents a request that failed with nothing further to say about why.
A disagreement the program noticed itself carries no refusal and no fault.
The headline alone is the whole answer, and an empty detail line under it would read as a missing message.

### `internal PWindowStored PWindowCommitRun<PWindowStored>(long held, Func<long, PWindowStored> commit)`

Runs one commit of a held draft, and handles the one refusal the user can answer.
When the engine refuses because a stored value cannot be read, a yes-or-no dialog asks whether to drop such values.
Yes has the engine drop them and runs the commit once more.
No, and every other failure, rethrows so the caller reports it as before.

### `internal void PWindowFailureShow(string key, Exception exception)`

Presents a request that failed, under the localized headline the given key names.
A deliberate refusal carries a reason key and resolves through the same catalog as the rest of the interface.

### `private string PWindowDetailRead(Exception exception)`

Reads the detail line out of a failure.

A refusal is a position the user can act on, so its reason is what they are shown.
Anything else is a fault.
A fault's own message is written for whoever fixes the program, not for whoever uses it.
A column name and an ordinal tell the user nothing they can act on.
The box says plainly that something unexpected went wrong.
The fault itself is written to the workspace's audit log and the box names the file.
So nothing diagnosable is lost, and nobody is handed a stack trace they did not ask for.

### `private string? PWindowRefusalRead(Exception exception)`

Whether a refusal stands anywhere inside the failure, and the reason it names.
A refusal is often wrapped by the step that was running when it was raised.
Reading only the outermost exception would turn a stated position into an unexplained fault.

### `private (Func<bool> Check, Func<bool, bool> Finish)[] PWindowEditorRead()`

The editors the window closes over, each paired with its own two answers.
Asking and finishing read the same list, so a panel cannot be asked about work the exit would not carry.
A panel whose edits have no store path yet belongs in neither.
A prompt that promises a save is worse than no prompt.
The corpus panel is on the list now that its sentence editor holds a draft it can commit.
The repertoire panel joins it on the same terms, its situation editor holding one too.
The reference panel completes the list, so every editor in the shell is both asked and finished.

### `internal bool PWindowDiscardConfirm()`

Asks before typed text is thrown away, and answers whether it may be.
Every editor on the list is asked whether it holds unsaved work.
Each is asked even after one has answered yes, because the asking is what writes a pause-held keystroke down.
The window closing and the workspace changing take all of them with it.
Unsaved work is put to the leave dialog, which offers three answers rather than two.
Discarding was the only exit, and a leaving user often means to keep the word.
Saving commits every draft the window holds and discarding cancels every one.
The third answer leaves the window standing.
Either answer that lets the window go empties the drafts folder of this window's files.
A leftover would otherwise be reported at the next launch as work a crash cost.
A clean exit never cost any.
A panel holding nothing unsaved is settled without a question, since its blank draft has nothing to lose either way.
Saving that the engine refuses answers no, so the window stays open over the entry it failed to store.

### `private bool PWindowDraftFinish((Func<bool> Check, Func<bool, bool> Finish)[] editors, bool store)`

Carries one answer to every editor on the list the question was put over.
Each holds its own draft under its own origin, so each is told separately.
An unchanged draft is cancelled whatever the answer, because storing what matches its entry would rewrite the entry for nothing.
Every editor is told before the answers are read.
One refusal does not leave the others still holding their drafts.
The window may go only when all of them are finished.

### `internal bool PWindowDiscardConfirm(bool unsaved, Func<bool, bool> finish)`

The same question over one panel's own answer, with the panel's own way of saving.
It is for a panel leaving its editing state while the rest of the window stays as it is.
Nothing unsaved means nothing to ask about.
So the question is only ever put when there is something to lose.
That is why it can sit in front of every path that discards typed work.
Saving runs the panel's finish and answers whether it went through, so a refused save keeps the panel put.
Discarding answers yes and leaves the dropping to the path that asked, as it always did.
A plain yes-or-no offered no way to keep the work, which a user leaving a word often means to keep.

### `internal bool PWindowRemovalConfirm(int usage, string scope)`

Asks before a shared record is deleted, and says how much the delete reaches.
A record nothing references is a plain question.
One something references names the number of places first, because the delete drops those references too.
`scope` names the kind of record, so a Situation and an Example each speak of themselves.
The wording is the panel's, but the shape of the question belongs to the window that asks it.
