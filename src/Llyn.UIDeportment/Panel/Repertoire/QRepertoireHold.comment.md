# QRepertoireHold.cs

## `internal sealed partial class QRepertoire`

When what the user typed into the situation editor reaches the draft the engine holds.
The panel holds its controls, and the desk on its deportment holds the tenure from start to commit or cancel.
The panel keeps no draft id, halted flag, timer or pending map of its own, since the engine owns them.
Every edit goes through one of the desk's gates, which build the request and defer or send it.
Typing is deferred through the tenure and written once the user stops, so a keystroke is not a write.
A media row added or removed is sent at once, because there is no keystroke coming to end it.
The tenure raises a bulletin when its state moves, and the deportment settles the buttons from that.

## `private LDesk QScenarioDesk => _lRepertoire.LRepertoireDesk;`

The desk holding the Situation being edited, read off the deportment each time.

## `private void QScenarioDeskAttach()`

Wires the desk's notices once, and a failure and a refused hold both go straight to the window.
The draft observer and the tenure observer, which updates the desk's state, are registered here too.

## `private void QScenarioChangeDefer()`

Hands the three texts to the text gate, for every edit the three fields report.
The engine decides what changed, and an unchanged body raises no bulletin, so nothing is redrawn under the caret.

## `private void QScenarioDraftRestore()`

Reads the held Situation back and redraws the controls from it.
This is what the panel's own draft bulletin does.
What is waiting is written first, so a bulletin from the tenure's timer never redraws over a newer keystroke.

## `public void PChronicleUndo()`

Steps whichever draft is in front one snapshot back, through the deportment.
The step runs inside `PChronicle.PChronicleRun`, so the caret stays at the end of the focused box.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void PChronicleRedo()`

Steps whichever draft is in front one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `public void PChronicleUpdate()`

Lights `QRepertoireBackward` and `QRepertoireForward` only when the draft in front has a step to walk.
The deportment reads the entry editor's chronicle while `PEditor` is in front, and the held Situation's otherwise.
The panel is the pair's only writer, so the two editors never overwrite each other.

## `private void QRepertoireUndoHandle(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.

## `private void QRepertoireRedoHandle(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
