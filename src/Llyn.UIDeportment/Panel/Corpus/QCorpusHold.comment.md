# QCorpusHold.cs

## `internal sealed partial class QCorpus`

When what the user typed into the sentence editor reaches the draft the engine holds.
The driver holds its controls.
The desk on the corpus Conduct holds the tenure from start to commit or cancel.
The driver keeps no draft id, halted flag or timer of its own, since the engine owns each of those.
Every change goes through the desk's text gate, which defers typing and sends a pick or a row at once.

## `private CDesk QTranscriptDesk => _qCorpusDesk;`

The desk holding the Example being edited, which the corpus Conduct handed over at introduce.

## `private LQuill? QTranscriptQuill => _cCorpus.CCorpusDesk.CDeskQuill;`

The desk's text edits, which build every request the transcript makes.
It is null while no tenure is held or the desk fills its controls.

## `private void QTranscriptDeskIntroduce()`

Subscribes the desk's notices once, and a failure and a refused hold both go straight to the window.
The corpus raises `CCorpusDraftChanged` with the held Example on each draft bulletin.
`QTranscriptShow` answers it and redraws the controls where they differ.
The forge built the corpus with its marshaller, so the driver hands no marshaller and no reread here.

## `public void QChronicleUndo()`

Steps whichever draft is in front one snapshot back, through the corpus session.
The step runs inside `QChronicle.QChronicleRun`, so the caret stays at the end of the focused box.

## `public void QChronicleRedo()`

Steps whichever draft is in front one snapshot forward again.

## `public void QChronicleUpdate()`

Lights `QCorpusBackward` and `QCorpusForward` only when the draft in front has a step to walk.
The session reads the entry editor's chronicle while `PEditor` is in front, and the held sentence's otherwise.

## `private void QCorpusUndoHandle(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.

## `private void QCorpusRedoHandle(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
