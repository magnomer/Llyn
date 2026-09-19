# PEditorHold.cs

## `public partial class PEditor`

The draft the form writes into, and what becomes of it.
The form holds one `LTenure` and its controls, and the tenure runs the hold from start to commit or cancel.
The form keeps no draft id, halted flag or timer of its own, since the engine owns each of those.
Starting one, reading it back, stepping it, and committing or throwing it away are all here.
The tenure raises a bulletin when its state moves, and the form settles its buttons from that.
This mirrors `PCorpusHold.cs` member for member, so the editors cannot drift apart.

## `private LTenure? _pEditorTenure;`

The engine's hold on the entry being edited, or null while the form holds nothing.
It is the form's only claim on anything outside itself.
The draft carries the entry it was started from, so the form no longer remembers that.

## `internal Action? PEditorChronicleNotice;`

Raised after every settle of the form's chronicle state, so the mounting panel settles its rail in turn.
The rail's pair follows the form's draft the way the rail's save follows the change notice.

## `private long PEditorDraft => _pEditorTenure?.LTenureId ?? 0;`

The held draft's id as the requests name it, zero while nothing is held.
Read from the tenure each time, so the form keeps no copy to drift.

## `private LDraft? PEditorDraftStart(long? entry)`

Hands the current draft back and asks the engine for a tenure on a new one.
Everything the form shows from then on belongs to that draft.
The card lists are emptied first, because a card's id is an address into the draft that is gone.
A stored entry reopened would otherwise find its old cards by id and keep their stale chips.
An entry that no longer loads is refused before a file is written, and the form comes up empty.
A refusal disables the form rather than leaving it typing into a tenure the engine never gave.
A start that succeeds enables the form again, since the downstream the form lost is back.

## `public void PChronicleUndo()`

Steps the entry form's draft one snapshot back through the tenure.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void PChronicleRedo()`

Steps the entry form's draft one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `private void PEditorUndoHandle(object sender, RoutedEventArgs e)`

The undo button, which walks the same road the keys walk.

## `private void PEditorRedoHandle(object sender, RoutedEventArgs e)`

The redo button, the inverse of the one above.

## `public void PChronicleUpdate()`

Lights the form's own undo and redo buttons only when the tenure has a step to walk.
No tenure disables both.
Then the chronicle notice is raised, so a panel that mounts the form can settle its own rail.
The form never touches a panel's buttons, because only the panel knows which editor is in front.

## `internal (bool PEditorPast, bool PEditorFuture) PEditorChronicleRead()`

The undo and redo answers of the tenure's state, both false while nothing is held.
A mounting panel reads this when the form is in front, the way it reads the change notice for save.

## `private void PEditorChronicleRun(Func<LTenure, LDraft?> step)`

The one path both steps share.
No tenure means nothing to walk, so it returns without asking.
The tenure writes what is waiting before it steps, so the snapshot stepped away from is the one on screen.
The step runs inside `PChronicle.PChronicleRun`, so the caret stays at the end of the focused box.
A step the engine refuses is shown as a hold failure, since the draft itself could not be reached.
The buttons are settled afterwards, since a step that found nothing raises no bulletin to settle them.

## `private void PEditorHoldShow(bool running)`

Enables or disables the form to match whether the tenure still runs, and says so once when it stops.
The whole editor is disabled, so no further keystroke is taken into a buffer that has nowhere to push.
The control's own enabled state is the memory of having said so, so the notice is not repeated.

## `private void PEditorDraftCancel()`

Throws the held draft away, links and all, and forgets the tenure.
The tenure is dropped before the call, so its last bulletin finds no form holding it.

## `private void PEditorDraftRestore()`

Reads the draft as stored and renders it over the form.
This is what every draft bulletin does, and what a form does when it doubts what it shows.
What is waiting is written first, so a bulletin from the tenure's timer never redraws over a newer keystroke.
A draft that reads back null leaves the form alone, since there is nothing left to agree with.

## `internal bool PEditorDraftFinish(bool store)`

The window's exit answer applied to this form's own draft.
The tenure writes what is waiting, cancels an unchanged or unwanted draft, and commits the rest.
So a form closed between two keystrokes carries the last of them out with it.
Storing commits it, which is the same write the save button makes.
A word typed and never saved survives the exit that was meant to keep it.
A refused commit keeps the tenure and reports the refusal, the same way the save button does.
A halted tenure refuses to finish the same way, since committing would store a draft missing the dropped edits.
What is returned is whether the form is finished.
A false answer holds the window open over work the store would not take.
A settled form leaves the folder no file, so a clean exit is never reported as work a crash cost.
