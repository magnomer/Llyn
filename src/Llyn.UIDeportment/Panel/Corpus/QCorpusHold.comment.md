# QCorpusHold.cs

## `internal sealed partial class QCorpus`

When what the user typed into the sentence editor reaches the draft the engine holds.
The driver holds its controls.
The desk on the corpus Conduct holds the tenure from start to commit or cancel.
The driver keeps no draft id, halted flag or timer of its own, since the engine owns each of those.
Every change goes through a gate on `CAnthology` or `CCorpus`.
Typing is deferred, and a pick or a row is sent at once.

## `private void QTranscriptDeskIntroduce()`

Subscribes the Conduct's draft notice once.
The desk shows its own failures and refused holds.
The corpus raises `CCorpusDraftChanged` with the held Example on each draft bulletin.
`QTranscriptDraftRefine` answers it and redraws the controls where they differ.
The introduce built the corpus with its marshal, so the driver hands no marshal and no reread here.

## `public void QChronicleUndoObserve()`

Steps whichever draft is in front one snapshot back, through the corpus session.
The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.

## `public void QChronicleRedoObserve()`

Steps whichever draft is in front one snapshot forward again.

## `private void QCorpusChronicleRefine()`

Lights `QCorpusBackward` and `QCorpusForward` only when the draft in front has a step to walk.
The session reads the entry editor's chronicle while `PEditor` is in front, and the held sentence's otherwise.

## `private void QCorpusUndoObserve(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.
It is the click's adapter to the chronicle step, which calls the session's gate.

## `private void QCorpusRedoObserve(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
