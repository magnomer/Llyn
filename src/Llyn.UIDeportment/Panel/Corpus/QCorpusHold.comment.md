# QCorpusHold.cs

## `internal sealed partial class QCorpus`

When what the user typed into the sentence editor reaches the draft the engine holds.
The driver holds its controls, and the desk on its deportment holds the tenure from start to commit or cancel.
The driver keeps no draft id, halted flag or timer of its own, since the engine owns each of those.
Every change goes through the desk's text gate, which defers typing and sends a pick or a row at once.

## `private LDesk QTranscriptDesk => _lCorpus.LCorpusDesk;`

The desk holding the Example being edited, read off the deportment each time.

## `private LQuill? QTranscriptQuill => _lCorpus.LCorpusDesk.LDeskQuill;`

The desk's text edits, which build every request the transcript makes.
It is null while no tenure is held or the desk fills its controls.

## `private void QTranscriptDeskAttach()`

Wires the desk's notices once, and a failure and a refused hold both go straight to the window.
The draft observer and the tenure observer are bound to the surface, since the driver is no control.

## `private void QTranscriptDraftRestore()`

Reads the held sentence back and redraws the controls from it where they differ.
This is what the desk's own draft bulletin does.
The sentence is read inline, so no driver local carries it.

## `public void QChronicleUndo()`

Steps whichever draft is in front one snapshot back, through the deportment.
The step runs inside `QChronicle.QChronicleRun`, so the caret stays at the end of the focused box.

## `public void QChronicleRedo()`

Steps whichever draft is in front one snapshot forward again.

## `public void QChronicleUpdate()`

Lights `QCorpusBackward` and `QCorpusForward` only when the draft in front has a step to walk.
The deportment reads the entry editor's chronicle while `PEditor` is in front, and the held sentence's otherwise.

## `private void QCorpusUndoHandle(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.

## `private void QCorpusRedoHandle(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
