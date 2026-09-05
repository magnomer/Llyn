# PInput.xaml.cs

## `public partial class PInput : UserControl`

The input panel as a control: the shared editor standing on no entry at all.
The lexical editing structure itself is `PEditor`, which every panel that edits an entry mounts.
This panel is what that editor means here.
It is the form an entry is created through, and only created.
It never opens on a stored entry.
So a store here always writes a new one and leaves the form empty for the next.
Correcting an entry that exists is the browse-style panels' work.

## `internal void PInputAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened, and opens the form empty.

## `internal void PInputReset()`

Empties the form.
The workspace folder can change while the window is up.
A different folder is a different database.
Whatever was typed against the old one is begun again here.

## `internal void PInputDraftFinish(bool store)`

Carries the window's exit answer down to the editor this panel owns.
The panel holds no draft of its own, so it only passes the answer along.

## `internal bool PInputChangeCheck()`

Whether the form differs from the one the user was given.
That is what the window asks before typed work would be thrown away.

## `internal void PInputClose()`

Stops the panel: the editor is shut down.

## Inline notes

### `PEditor.PEditorAttach(host, engine, "Input", null);`

No entry: this panel creates them.
The origin names this panel, so its held work is told apart from the library's.
A form that stood on one would turn the next store into an update of it.
That is how a session's second entry used to overwrite its first.

### `PEditor.PEditorDiscardDispatcher = PEditor.PEditorReset;`

Discarding empties the form.
This panel is where an entry is begun, not where a stored one is corrected.
So the typing is what is thrown away rather than reverted.
