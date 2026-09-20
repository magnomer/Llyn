# PRepertoireHold.cs

## `public partial class PRepertoire`

When what the user typed into the situation editor reaches the draft the engine holds.
The panel holds its controls, and the desk on its deportment holds the tenure from start to commit or cancel.
The panel keeps no draft id, halted flag, timer or pending map of its own, since the engine owns them.
Typing is deferred through the tenure and written once the user stops, so a keystroke is not a write.
A media row added or removed is applied at once, because there is no keystroke coming to end it.
The tenure raises a bulletin when its state moves, and the panel settles its buttons from that.
This mirrors `PCorpusHold.cs` member for member, so the editors cannot drift apart.

## `private LDesk PScenarioDesk => _lRepertoire.LRepertoireDesk;`

The desk holding the Situation being edited, read off the deportment each time.

## `private void PScenarioDeskAttach()`

Wires the desk's notices once: the start, a failure to show and the stored id to show.
The draft and state observers are registered on the desk here too.

## `private void PScenarioStartUpdate()`

Enables the editor once the desk holds a tenure.
The panel's draft and state observers were registered on the desk at attach time.

## `private void PScenarioFailureShow(string key, Exception exception)`

Shows a desk failure under the key the desk named, the load key or the save key of its scope.

## `private long PScenarioDraft => PScenarioDesk.LDeskId;`

The held draft's id as the requests name it, zero while nothing is held.
Read from the desk each time, so the panel keeps no copy to drift.

## `private bool PScenarioDraftFinish(bool store)`

Ends the held Situation when the panel is left, committing it or discarding it.
The panel routes here only while the Situation editor is the side in front.
The tenure writes what is waiting, cancels an unchanged or unwanted draft, and commits the rest.
A refused commit keeps the tenure and answers false, so the window stays open over work still on disk.
A halted tenure refuses to finish the same way, since committing would store a draft missing the dropped edits.

## `private bool PScenarioChangeCheck()`

Whether the editor holds work a host would be sorry to lose.
Typing still waiting to be written is written first.
Otherwise a window closing within a keystroke of the last change would call it unchanged.

## `private void PScenarioChangeDefer()`

Defers the whole body of the form as one request, for every edit the three fields report.
The engine decides what changed, and an unchanged body raises no bulletin, so nothing is redrawn under the caret.
The body names no situation id, since the engine lands it on the Situation the draft holds.

## `private void PScenarioRequestDefer(LRequest request)`

Hands one request to the tenure to write once the typing stops.
A filling panel and one holding no draft defer nothing.
A halted tenure drops the request itself.
A media row's location travels this way, keyed by its row, so a later edit replaces the earlier one waiting.

## `private void PScenarioRequestSend(LRequest request)`

Hands one request to the tenure to write now, for a media row added or removed.
What was waiting is written first, so a location typed before the add travels ahead of it.

## `private void PScenarioChangeUpdate()`

Settles the rail's save, the editor's enabled state and the undo pair from one reading of the tenure's state.
It reads the same answer the closing warning reads, so the two cannot disagree.
It stands aside while `PEditor` is in front, because then the save belongs to the entry editor.
With no tenure the editor keeps whatever enabled state a failed start left it, since there is nothing to read.

## `private void PScenarioHoldShow(bool running)`

Enables or disables the editor to match whether the tenure still runs, and says so once when it stops.
Editing on would collect keystrokes nothing is holding, which is the loss the draft exists to prevent.
The control's own enabled state is the memory of having said so, so the notice is not repeated.

## `private LDraft? PScenarioDraftStart(long? situation)`

Starts the desk on a stored Situation or on nothing, and hands back the draft it holds.
The desk discards whatever was held before, so the panel never holds two.
A refusal leaves the desk empty and disables the editor rather than leaving it typing into nothing.

## `private void PScenarioDraftShow(LDraft? started)`

Fills the controls from a draft just started, or empties them when none was.

## `private void PScenarioDraftRestore()`

Reads the held Situation back and redraws the controls from it where they differ.
This is what the panel's own draft bulletin does.
What is waiting is written first, so a bulletin from the tenure's timer never redraws over a newer keystroke.
Nothing is read while the controls are being filled, because filling raises the bulletin's own echo.

## `private void PScenarioDraftCancel()`

Discards the held Situation and forgets the tenure.
The tenure is dropped before the call, so its last bulletin finds no panel holding it.

## `private long? PScenarioSituationRead()`

The stored Situation the held work was opened on, or null for one nothing has stored.
The delete control, the usage count and the discard all read identity through this.

## `public void PChronicleUndo()`

Steps the situation form's draft one snapshot back through the tenure.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void PChronicleRedo()`

Steps the situation form's draft one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `private void PScenarioChronicleRun(Action step)`

The one path both steps share.
The desk walks nothing while it holds nothing.
The step runs inside `PChronicle.PChronicleRun`, so the caret stays at the end of the focused box.
A step the engine refuses is shown as a hold failure, since the draft itself could not be reached.
The buttons are settled afterwards, since a step that found nothing raises no bulletin to settle them.

## `public void PChronicleUpdate()`

Lights `PRepertoireBackward` and `PRepertoireForward` only when the tenure has a step to walk.
No tenure disables both.
While `PEditor` is in front the pair follows that editor's draft instead, read through `PEditorChronicleRead`.
The panel is the pair's only writer, so the two editors never overwrite each other.
The editor's chronicle notice and the editor toggle both call here, so the pair follows whoever is in front.

## `private void PRepertoireUndoHandle(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.
An Entry open in `PEditor` steps its own chronicle, and otherwise the held draft steps.

## `private void PRepertoireRedoHandle(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
