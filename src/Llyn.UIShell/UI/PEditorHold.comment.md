# PEditorHold.cs

## `public partial class PEditor`

The draft the form writes into, and what becomes of it.
The form keeps the id of the draft the engine holds for it, and writes every keystroke through that.
Starting one, writing into it, reading it back, and committing or throwing it away are all here.
A downstream that refuses any of those takes the form out of the user's hands until one answers again.

## Inline notes

### `private long _pEditorDraft;`

Which held draft this form is editing.
It is the form's only claim on anything outside itself.
The draft carries the entry it was started from, so the form no longer remembers that.
Zero means the engine refused to start one, and every write here is skipped.

### `private LDraft? PEditorDraftStart(long? entry)`

Hands the current draft back and asks the engine for a new one.
Everything the form shows from then on belongs to that draft.
The card lists are emptied first, because a card's id is an address into the draft that is gone.
A stored entry reopened would otherwise find its old cards by id and keep their stale chips.
An entry that no longer loads is refused before a file is written, and the form comes up empty.
A refusal is reported and the form is suspended.
A control may buffer keystrokes only while a draft waits for them.
A draft that starts lifts a suspension, since the downstream the form lost is back.

### `private void PEditorHoldSuspend(Exception exception)`

Puts the form into a held-nothing state and says why once.
The whole editor is disabled, so no further keystroke is taken into a buffer that has nowhere to push.
Only the first failure is reported, because a reset after one raises the same failure again.

### `private void PEditorHoldResume()`

Gives the form back to the user once a draft is holding its keystrokes again.
It does nothing to a form that was never suspended, so an ordinary start touches no control.

### `private void PEditorDraftCancel()`

Throws the held draft away, links and all.
The id is dropped first, so a failure to delete cannot leave the form writing into a dead draft.
A failure to delete is reported, since a draft left behind is offered back as leftover on the next launch.

### `private void PEditorDraftRestore()`

Reads the draft as stored and renders it over the form.
This is what every draft bulletin does, and what a form does when it doubts what it shows.
A draft that reads back null leaves the form alone, since there is nothing left to agree with.

### `internal bool PEditorDraftFinish(bool store)`

The window's exit answer applied to this form's own draft.
A write still waiting on the typing pause is made before the wait is dropped.
So a form closed between two keystrokes carries the last of them out with it.
The caller is not trusted to have asked the form for changes first.
Storing commits it, which is the same write the save button makes.
A word typed and never saved survives the exit that was meant to keep it.
Discarding cancels it.
A draft matching its entry is cancelled either way, since there is nothing in it to store.
A refused commit keeps the draft and reports the refusal, the same way the save button does.
A form halted by a failed flush answers that it did not finish.
Committing then would store a draft missing the edits the flush dropped.
The answer the user gave was to keep the word.
Deleting it is the one thing that answer never asked for.
What is returned is whether the form is finished.
A false answer holds the window open over work the store would not take.
A settled form leaves the folder no file, so a clean exit is never reported as work a crash cost.

### `private bool PEditorDraftCheck()`

Asks the engine whether the held draft differs from the entry it started from, with the refusal left unread.

### `private bool PEditorDraftCheck(out string? refusal)`

Asks the engine whether the held draft differs from the entry it started from.
`refusal` is the reason a commit would be refused now, or null, which is what enables the save button.
A form with no draft has nothing to lose, so it answers no.
A question the engine cannot answer at all suspends the form.
The draft behind it can no longer be trusted.
