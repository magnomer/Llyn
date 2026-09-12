# PCorpusHold.cs

## `public partial class PCorpus`

When what the user typed into the sentence editor reaches the draft the engine holds.
The panel no longer keeps a copy of the stored Example to compare itself against.
It writes what it holds instead, and asks the engine whether that differs.
Typing is written once the user stops, because a write per keystroke is a write per keystroke.
A citation or a language chosen writes at once, because there is no keystroke coming to end it.
This mirrors `PEditorChange.cs` and `PEditorDraft.cs` field for field, so the two editors cannot drift apart.

## `internal bool PCorpusDraftFinish(bool store)`

Ends the held sentence when the window closes, committing it or discarding it.
Typing still waiting to be written is written first, whatever the answer was.
A refused commit puts the id back and answers false, so the window stays open over work still on disk.

## `private bool PTranscriptChangeCheck()`

Whether the editor holds work a host would be sorry to lose.
Typing still waiting to be written is written first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## `private void PTranscriptChangeDefer()`

Restarts the wait that ends in a write, for every edit the controls report.
A filling panel, a suspended one, and one holding no draft each write nothing.

## `private async Task PTranscriptChangeRun(CancellationToken token)`

Waits out the quiet and then writes, unless another edit cancels the wait first.
The wait is not awaited, because the keystroke that started it must return at once.
Its continuation comes back on the UI thread, which is where the engine is used.
Nobody is left to observe the task, so the write it ends with is guarded inside it.

## `private void PTranscriptChangeSave()`

The one place control values reach the held sentence outside a commit.
It also settles the buttons, so a write and what the buttons say never drift apart.

## `private void PTranscriptChangeUpdate()`

Settles the discard and store controls against the engine's answer.
Both read the same answer the closing warning reads, so the three cannot disagree.

## `private LDraft? PTranscriptDraftStart(long? example)`

Starts a held sentence, on a stored Example or on nothing, and hands back what was started.
Whatever was held before is discarded first, so the panel never holds two.
A refusal suspends the editor rather than leaving it typing into an id the engine never gave.

## `private void PTranscriptDraftShow(LDraft? started)`

Fills the controls from a draft just started, or empties them when none was.

## `private void PTranscriptDraftSave()`

Sends the whole body of the form as one request and lets the engine decide what changed.
An unchanged body is dropped by the engine without a bulletin, so nothing is redrawn under the caret.
A changed body answers with one bulletin, and the bulletin redraws only what differs.

## `private void PTranscriptDraftRestore()`

Reads the held sentence back and redraws the controls from it where they differ.
This is what the panel's own draft bulletin does.
Nothing is read while the controls are being filled, because filling raises the bulletin's own echo.

## `private void PTranscriptDraftCancel()`

Discards the held sentence and forgets its id.
The id is dropped before the call, so a failing discard cannot leave the panel writing into it.
A failure here is swallowed, because the caller is already leaving the work behind.

## `private bool PTranscriptDraftCheck()`

The engine's answer to whether the held sentence differs from the Example it opened on.
A panel holding no draft has nothing to lose and answers false.

## `private long? PTranscriptExampleRead()`

The stored Example the held sentence was opened on, or null for one nothing has stored.
The delete control, the usage count and the discard all read identity through this.

## `private void PTranscriptHoldSuspend(Exception exception)`

Stops the editor when the push breaks, and says so once.
Editing on would collect keystrokes nothing is holding, which is the loss the draft exists to prevent.

## `private void PTranscriptHoldResume()`

Gives the editor back once a draft is held again.

## Inline notes

### `private const int PTranscriptChangeDelay = 250;`

Long enough that ordinary typing writes once rather than once per letter.
Short enough that a crash costs a word, not a sentence.

### `private const string PTranscriptOrigin = "Corpus";`

The surface a recovered sentence says it came from.
