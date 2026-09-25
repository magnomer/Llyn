# PCorpusHold.cs

## `public partial class PCorpus`

When what the user typed into the sentence editor reaches the draft the engine holds.
The panel holds its controls, and the desk on its deportment holds the tenure from start to commit or cancel.
The panel keeps no draft id, halted flag or timer of its own, since the engine owns each of those.
Typing is deferred through the tenure and written once the user stops, so a keystroke is not a write.
A row added or removed is applied at once, because there is no keystroke coming to end it.
The tenure raises a bulletin when its state moves, and the deportment settles the buttons from that.

## `private LDesk PTranscriptDesk => _lCorpus.LCorpusDesk;`

The desk holding the Example being edited, read off the deportment each time.

## `private void PTranscriptDeskAttach()`

Wires the desk's notices once, and a failure and a refused hold both go straight to the window.
The draft observer and the tenure observer, which updates the desk's state, are registered here too.

## `private long PTranscriptDraft => PTranscriptDesk.LDeskId;`

The held draft's id as the requests name it, zero while nothing is held.
Read from the desk each time, so the panel keeps no copy to drift.

## `private void PTranscriptRequestDefer(LRequest request)`

Hands one request to the tenure to write once the typing stops.
A filling panel and one holding no draft defer nothing.
A halted tenure drops the request itself.

## `private void PTranscriptRequestSend(LRequest request)`

Hands one request to the tenure to write now, for a Mention or a Gloss alike.
What was waiting is written first, because a Mention span was measured against the text as typed.
The bulletin that answers redraws the transcript, chip line included.

## `private void PTranscriptDraftRestore()`

Reads the held sentence back and redraws the controls from it where they differ.
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

Lights `PCorpusBackward` and `PCorpusForward` only when the draft in front has a step to walk.
The deportment reads the entry editor's chronicle while `PEditor` is in front, and the held sentence's otherwise.
The panel is the pair's only writer, so the two editors never overwrite each other.

## `private void PCorpusUndoHandle(object sender, RoutedEventArgs e)`

The rail's undo, standing for whichever editor is in front, as the rail's save does.

## `private void PCorpusRedoHandle(object sender, RoutedEventArgs e)`

The rail's redo, the inverse of the one above.
