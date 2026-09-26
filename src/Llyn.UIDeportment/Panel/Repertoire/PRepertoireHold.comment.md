# PRepertoireHold.cs

## `public partial class PRepertoire`

When what the user typed into the situation editor reaches the draft the engine holds.
The panel holds its controls, and the desk on its deportment holds the tenure from start to commit or cancel.
The panel keeps no draft id, halted flag, timer or pending map of its own, since the engine owns them.
Typing is deferred through the tenure and written once the user stops, so a keystroke is not a write.
A media row added or removed is applied at once, because there is no keystroke coming to end it.
The tenure raises a bulletin when its state moves, and the deportment settles the buttons from that.
This mirrors `PCorpusHold.cs` member for member, so the editors cannot drift apart.
It adds `PScenarioChangeDefer` and the loading guard in `PScenarioRequestSend`, which the corpus lacks.

## `private LDesk PScenarioDesk => _lRepertoire.LRepertoireDesk;`

The desk holding the Situation being edited, read off the deportment each time.

## `private void PScenarioDeskAttach()`

Wires the desk's notices once, and a failure and a refused hold both go straight to the window.
The draft observer and the tenure observer, which updates the desk's state, are registered here too.

## `private long PScenarioDraft => PScenarioDesk.LDeskId;`

The held draft's id as the requests name it, zero while nothing is held.
Read from the desk each time, so the panel keeps no copy to drift.

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
A filling panel sends nothing, because filling is the draft's own echo and not a user's act.

## `private void PScenarioDraftRestore()`

Reads the held Situation back and redraws the controls from it where they differ.
This is what the panel's own draft bulletin does.
What is waiting is written first, so a bulletin from the tenure's timer never redraws over a newer keystroke.
Nothing is read while the controls are being filled, because filling raises the bulletin's own echo.

## `public void PChronicleUndo()`

Steps whichever draft is in front one snapshot back, through the deportment.
The step runs inside `PChronicle.PChronicleRun`, so the caret stays at the end of the focused box.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void PChronicleRedo()`

Steps whichever draft is in front one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `public void PChronicleUpdate()`

Lights `PRepertoireBackward` and `PRepertoireForward` only when the draft in front has a step to walk.
The deportment reads the entry editor's chronicle while `PEditor` is in front, and the held Situation's otherwise.
The panel is the pair's only writer, so the two editors never overwrite each other.

## `private void PRepertoireUndoHandle(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.

## `private void PRepertoireRedoHandle(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
