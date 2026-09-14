# PRepertoireHold.cs

## `public partial class PRepertoire`

When what the user typed into the situation editor reaches the draft the engine holds.
The panel no longer keeps a copy of the stored Situation to compare itself against.
It writes what it holds instead, and asks the engine whether that differs.
Typing is written once the user stops, because a write per keystroke is a write per keystroke.
This mirrors `PCorpusHold.cs` field for field, so the three editors cannot drift apart.

## `private bool PScenarioDraftFinish(bool store)`

Ends the held Situation when the panel is left, committing it or discarding it.
The panel routes here only while the Situation editor is the side in front.
Typing still waiting to be written is written first, whatever the answer was.
A refused commit puts the id back and answers false, so the window stays open over work still on disk.

## `private bool PScenarioChangeCheck()`

Whether the editor holds work a host would be sorry to lose.
Typing still waiting to be written is written first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## `private void PScenarioChangeDefer()`

Restarts the wait that ends in a write, for every edit the controls report.
A filling panel, a suspended one, and one holding no draft each write nothing.

## `private async Task PScenarioChangeRun(CancellationToken token)`

Waits out the quiet and then writes, unless another edit cancels the wait first.
The wait is not awaited, because the keystroke that started it must return at once.
Its continuation comes back on the UI thread, which is where the engine is used.
Nobody is left to observe the task, so the write it ends with is guarded inside it.

## `private void PScenarioChangeSave()`

The one place control values reach the held Situation outside a commit.
It also settles the buttons, so a write and what the buttons say never drift apart.

## `private void PScenarioChangeUpdate()`

Settles the rail's save button against the engine's answer, while the Situation editor is the side in front.
It reads the same answer the closing warning reads, so the two cannot disagree.
While the entry editor is in front, that editor's own notice drives the same button.
This one then leaves it alone.

## `private LDraft? PScenarioDraftStart(long? situation)`

Starts a held Situation, on a stored one or on nothing, and hands back what was started.
Whatever was held before is discarded first, so the panel never holds two.
A refusal suspends the editor rather than leaving it typing into an id the engine never gave.

## `private void PScenarioDraftShow(LDraft? started)`

Fills the controls from a draft just started, or empties them when none was.

## `private void PScenarioDraftSave()`

Sends the whole body of the form as one request and lets the engine decide what changed.
An unchanged body is dropped by the engine without a bulletin, so nothing is redrawn under the caret.
A changed body answers with one bulletin, and the bulletin redraws only what differs.
Row requests waiting from the media lists are written first, so a location typed with the description travels with it.
A failed write drops what was waiting, because the editor is suspended and nothing would send it.

## `private void PScenarioDraftRestore()`

Reads the held Situation back and redraws the controls from it where they differ.
This is what the panel's own draft bulletin does.
Nothing is read while the controls are being filled, because filling raises the bulletin's own echo.

## `private void PScenarioDraftCancel()`

Discards the held Situation and forgets its id, with any row request still waiting on it and the wait itself.
The id is dropped before the call, so a failing discard cannot leave the panel writing into it.
A failure here is swallowed, because the caller is already leaving the work behind.

## `private bool PScenarioDraftCheck()`

The engine's answer to whether the held Situation differs from the one it opened on.
A panel holding no draft has nothing to lose and answers false.

## `private long? PScenarioSituationRead()`

The stored Situation the held work was opened on, or null for one nothing has stored.
The delete control and the discard both read identity through this.

## `private void PScenarioHoldSuspend(Exception exception)`

Stops the editor when the push breaks, and says so once.
Editing on would collect keystrokes nothing is holding, which is the loss the draft exists to prevent.

## `private void PScenarioHoldResume()`

Gives the editor back once a draft is held again.

## Inline notes

### `private const int PScenarioChangeDelay = 250;`

Long enough that ordinary typing writes once rather than once per letter.
Short enough that a crash costs a word, not a description.

### `private const string PScenarioOrigin = "Repertoire";`

The surface a recovered Situation says it came from.
