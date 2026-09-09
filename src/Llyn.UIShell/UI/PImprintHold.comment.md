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

## `private void PImprintChangeUpdate()`

Settles the discard and store controls against the engine's answer.
Both read the same answer the closing warning reads, so the three cannot disagree.

## `private LDraft? PImprintDraftStart(string? reference)`

Starts a held Source, on a stored one or on nothing, and hands back what was started.
Whatever was held before is discarded first, so the panel never holds two.
A refusal suspends the editor rather than leaving it typing into an id the engine never gave.

## `private void PImprintDraftShow(LDraft? started)`

Fills the controls from a draft just started, or empties them when none was.

## `private void PImprintDraftSave()`

Writes the control values onto the held Source and renders what was stored.
The engine hands back what it stored, which is not always what was sent.
The controls are rebuilt only when those differ, or every keystroke would rebuild them under the caret.
A draft the engine no longer holds is left alone rather than recreated.

## `internal void PImprintDraftCancel()`

Discards the held Source and forgets its id.
The id is dropped before the call, so a failing discard cannot leave the panel writing into it.
A failure here is swallowed, because the caller is already leaving the work behind.

## `private bool PImprintDraftCheck()`

The engine's answer to whether the held Source differs from the one it opened on.
A panel holding no draft has nothing to lose and answers false.

## `private string? PImprintReferenceRead()`

The stored Source the held work was opened on, or null for one nothing has stored.
The delete control, the citation figure and every credit call read identity through this.
Credits are attached to a stored row, which is why they are offered only once this answers.

## `private void PImprintHoldSuspend(Exception exception)`

Stops the editor when the push breaks, and says so once.
Editing on would collect keystrokes nothing is holding, which is the loss the draft exists to prevent.

## `private void PImprintHoldResume()`

Gives the editor back once a draft is held again.

## Inline notes

### `private const int PImprintChangeDelay = 250;`

Long enough that ordinary typing writes once rather than once per letter.
Short enough that a crash costs a word, not a citation.

### `private const string PImprintOrigin = "Reference";`

The surface a recovered Source says it came from.
