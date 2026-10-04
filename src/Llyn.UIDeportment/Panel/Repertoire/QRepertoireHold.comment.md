# QRepertoireHold.cs
Hash: `b4b5a8c5bd14dccd`

## `internal sealed partial class QRepertoire`

When what the user typed into the situation editor reaches the draft the engine holds.
The panel holds its controls, and the desk on its playwright holds the tenure from start to commit or cancel.
The panel keeps no draft id, halted flag, timer or pending map of its own, since the engine owns them.
Every edit goes through one of the Conduct's gates, whose engine member builds the request and defers or sends it.
Typing is deferred through the tenure and written once the user stops, so a keystroke is not a write.
A media row added or removed is sent at once, because there is no keystroke coming to end it.
The tenure raises a bulletin when its state moves, and the Conduct settles the buttons from that.

## `private void QScenarioDeskIntroduce()`

Wires the playwright's draft notice to `QScenarioFieldsRefine`.
The desk shows its own failures and refused holds.
The playwright attaches the desk's observers itself at build, through the marshal `QRepertoire` hands it.

## `public void QChronicleUndoObserve()`

Steps whichever draft is in front one snapshot back, through the Conduct.
The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void QChronicleRedoObserve()`

Steps whichever draft is in front one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `private void QRepertoireChronicleRefine()`

Lights `QRepertoireBackward` and `QRepertoireForward` only when the draft in front has a step to walk.
The Conduct reads the entry editor's chronicle while `PEditor` is in front, and the held Situation's otherwise.
The panel is the pair's only writer, so the two editors never overwrite each other.

## `private void QRepertoireUndoObserve(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.

## `private void QRepertoireRedoObserve(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
