# PImprintHold.cs

## `public partial class PImprint`

When what the user typed into the source editor reaches the draft the engine holds.
The area holds one `LTenure` and its controls, and the tenure runs the hold from start to commit or cancel.
The area keeps no draft id, halted flag or timer of its own, since the engine owns each of those.
Typing is deferred through the tenure and written once the user stops, so a keystroke is not a write.
A credit added, moved or dropped is applied at once, because there is no keystroke coming to end it.
The tenure raises a bulletin when its state moves, and the area settles the panel's rail from that.
This mirrors `PCorpusHold.cs` member for member, so the editors cannot drift apart.

## `private const string PImprintOrigin = "Reference";`

The surface a recovered Source says it came from.

## `private LTenure? _pImprintTenure;`

The engine's hold on the Source being edited, or null while the area holds nothing.

## `internal Action? PImprintChronicleNotice;`

Raised after every settle of the area's chronicle state, so the panel settles its rail in turn.

## `private long PImprintDraft => _pImprintTenure?.LTenureId ?? 0;`

The held draft's id as the requests name it, zero while nothing is held.
Read from the tenure each time, so the area keeps no copy to drift.

## `internal bool PImprintDraftFinish(bool store)`

Ends the held Source when the window closes, committing it or discarding it.
The tenure writes what is waiting, cancels an unchanged or unwanted draft, and commits the rest.
A refused commit keeps the tenure and answers false, so the window stays open over work still on disk.
A halted tenure refuses to finish the same way, since committing would store a draft missing the dropped edits.

## `internal bool PImprintChangeCheck()`

Whether the editor holds work a host would be sorry to lose.
Typing still waiting to be written is written first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## `private void PImprintChangeDefer()`

Defers the whole body of the form as one request, for every edit the five fields report.
The engine decides what changed, and an unchanged body raises no bulletin, so nothing is redrawn under the caret.

## `private void PImprintRequestDefer(LRequest request)`

Hands one request to the tenure to write once the typing stops.
A filling area and one holding no draft defer nothing.
A halted tenure drops the request itself.

## `private bool PImprintRequestSend(LRequest request)`

Hands one credit request to the tenure to write now.
What was waiting is written first, so the requests reach the engine in order.
Says whether the tenure still runs afterwards, so a second request can depend on the first.
A refused credit halts the tenure, and the halt is shown once through the state bulletin.

## `internal void PImprintChangeUpdate()`

Hands the tenure's answer to the panel, which settles the rail's save on it.
It reads the same answer the closing warning reads, so the two cannot disagree.
The panel calls it when it brings the area back in front, so the save follows what is shown.
The area's enabled state and the undo pair are settled from the same reading.
With no tenure the area keeps whatever enabled state a failed start left it, since there is nothing to read.

## `private void PImprintHoldShow(bool running)`

Enables or disables the area to match whether the tenure still runs, and says so once when it stops.
Editing on would collect keystrokes nothing is holding, which is the loss the draft exists to prevent.
The control's own enabled state is the memory of having said so, so the notice is not repeated.

## `private LDraft? PImprintDraftStart(long? reference)`

Starts a tenure on a stored Source or on nothing, and hands back the draft it holds.
Whatever was held before is discarded first, so the area never holds two.
A refusal disables the area rather than leaving it typing into a tenure the engine never gave.

## `private void PImprintDraftShow(LDraft? started)`

Fills the controls from a draft just started, or empties them when none was.

## `internal void PImprintDraftRestore(long id)`

Redraws from the draft when `id` names the one this area holds, and ignores any other draft's bulletin.

## `private void PImprintDraftRestore()`

Reads the held Source back and redraws the controls from it where they differ.
This is what the panel's own draft bulletin does.
What is waiting is written first, so a bulletin from the tenure's timer never redraws over a newer keystroke.
Nothing is read while the controls are being filled, because filling raises the bulletin's own echo.

## `internal void PImprintDraftCancel()`

Discards the held Source and forgets the tenure.
The tenure is dropped before the call, so its last bulletin finds no area holding it.

## `private long? PImprintReferenceRead()`

The stored Source the held work was opened on, or null for one nothing has stored.
The delete control, the citation figure and every credit call read identity through this.
Credits are attached to a stored row, which is why they are offered only once this answers.

## `public void PChronicleUndo()`

Steps the source form's draft one snapshot back through the tenure.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void PChronicleRedo()`

Steps the source form's draft one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `private void PImprintChronicleRun(Func<LTenure, LDraft?> step)`

The one path both steps share.
No tenure means nothing to walk, so it returns without asking.
The tenure writes what is waiting before it steps, so the snapshot stepped away from is the one on screen.
The step runs inside `PChronicle.PChronicleRun`, so the caret stays at the end of the focused box.
A step the engine refuses is shown as a hold failure, since the draft itself could not be reached.
The buttons are settled afterwards, since a step that found nothing raises no bulletin to settle them.

## `public void PChronicleUpdate()`

Raises the chronicle notice, so the panel settles its rail from the draft held here.
The area owns no buttons of its own and never writes the panel's.
Only the panel knows which editor is in front.

## `internal (bool PImprintPast, bool PImprintFuture) PImprintChronicleRead()`

The undo and redo answers of the tenure's state, both false while nothing is held.
