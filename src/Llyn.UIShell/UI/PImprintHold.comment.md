# PImprintHold.cs

## `public partial class PImprint`

When what the user typed into the source editor reaches the draft the engine holds.
The panel no longer keeps a copy of the stored Source to compare itself against.
It writes what it holds instead, and asks the engine whether that differs.
Typing is written once the user stops, because a write per keystroke is a write per keystroke.
This mirrors `PRepertoireHold.cs` field for field, so the four editors cannot drift apart.

## `internal bool PImprintDraftFinish(bool store)`

Ends the held Source when the window closes, committing it or discarding it.
Typing still waiting to be written is written first, whatever the answer was.
A refused commit puts the id back and answers false, so the window stays open over work still on disk.
A form halted by a failed flush answers that it did not finish.
Committing then would store a draft missing the edits the flush dropped.

## `internal bool PImprintChangeCheck()`

Whether the editor holds work a host would be sorry to lose.
Typing still waiting to be written is written first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## `private void PImprintChangeDefer()`

Restarts the wait that ends in a write, for every edit the controls report.
A filling area, a suspended one, and one holding no draft each write nothing.

## `private async Task PImprintChangeRun(CancellationToken token)`

Waits out the quiet and then writes, unless another edit cancels the wait first.
The wait is not awaited, because the keystroke that started it must return at once.
Its continuation comes back on the UI thread, which is where the engine is used.
Nobody is left to observe the task, so the write it ends with is guarded inside it.

## `private void PImprintChangeSave()`

The one place control values reach the held Source outside a commit.
It also settles the buttons, so a write and what the buttons say never drift apart.

## `internal void PImprintChangeUpdate()`

Hands the engine's answer to the panel, which settles the rail's save on it.
It reads the same answer the closing warning reads, so the two cannot disagree.
The panel calls it when it brings the area back in front.
So the save is settled for the area shown.
The undo and redo pair is settled last, so it never says more than the draft can do.

## `private LDraft? PImprintDraftStart(long? reference)`

Starts a held Source, on a stored one or on nothing, and hands back what was started.
Whatever was held before is discarded first, so the panel never holds two.
A refusal suspends the editor rather than leaving it typing into an id the engine never gave.

## `private void PImprintDraftShow(LDraft? started)`

Fills the controls from a draft just started, or empties them when none was.

## `private void PImprintDraftSave()`

Sends the whole body of the form as one request and lets the engine decide what changed.
An unchanged body is dropped by the engine without a bulletin, so nothing is redrawn under the caret.
A changed body answers with one bulletin, and the bulletin redraws only what differs.

## `internal void PImprintDraftRestore(long id)`

Redraws from the draft when `id` names the one this area holds, and ignores any other draft's bulletin.

## `private void PImprintDraftRestore()`

Reads the held Source back and redraws the controls from it where they differ.
This is what the panel's own draft bulletin does.
Nothing is read while the controls are being filled, because filling raises the bulletin's own echo.

## `internal void PImprintDraftCancel()`

Discards the held Source and forgets its id.
The id is dropped before the call, so a failing discard cannot leave the panel writing into it.
A failure here is swallowed, because the caller is already leaving the work behind.

## `private bool PImprintDraftCheck()`

The engine's answer to whether the held Source differs from the one it opened on.
A panel holding no draft has nothing to lose and answers false.

## `private long? PImprintReferenceRead()`

The stored Source the held work was opened on, or null for one nothing has stored.
The delete control, the citation figure and every credit call read identity through this.
Credits are attached to a stored row, which is why they are offered only once this answers.

## `public void PChronicleUndo()`

Steps the source form's draft one snapshot back through the engine's chronicle.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void PChronicleRedo()`

Steps the source form's draft one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `public void PChronicleUpdate()`

Raises the chronicle notice, so the panel settles its rail from the draft held here.
The area owns no buttons of its own and never writes the panel's.
Only the panel knows which editor is in front.

## `internal (bool PImprintPast, bool PImprintFuture) PImprintChronicleRead()`

Whether the held draft has a step behind it and a step ahead of it.
No draft answers false to both.

## `private void PImprintChronicleRun(Func<long, LDraft?> step)`

The one path both steps share.
No draft means nothing to walk, so it returns without asking.
The pending debounced request is flushed first, so the snapshot stepped away from is the one on screen.
The step runs inside `PChronicle.PChronicleRun`, so the caret stays at the end of the focused box.
A step the engine refuses is shown as a hold failure, since the draft itself could not be reached.
The buttons are settled afterwards, since a step that found nothing raises no bulletin to settle them.

## `private void PImprintHoldSuspend(Exception exception)`

Stops the editor when the push breaks, and says so once.
Editing on would collect keystrokes nothing is holding, which is the loss the draft exists to prevent.

## `private void PImprintHoldResume()`

Gives the editor back once a draft is held again.

## Inline notes

### `internal Action? PImprintChronicleNotice;`

Raised after every settle of the area's chronicle state, so the panel settles its rail in turn.

### `private const int PImprintChangeDelay = 250;`

Long enough that ordinary typing writes once rather than once per letter.
Short enough that a crash costs a word, not a citation.

### `private const string PImprintOrigin = "Reference";`

The surface a recovered Source says it came from.
