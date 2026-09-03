# PTag.xaml.cs

## `public partial class PTag : UserControl`

The tag panel as a control: what it is made of, and when it starts and stops.
Browsing itself lives in the file beside this one.
That is the tag search, the tag ordering, the tag catalog, the entries under a tag, and the reader and editor beside them.

## `internal void PTagAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened.
Nothing is read yet.
The panel fills itself the first time it is shown.
So a session that never opens the tag tab never queries the database.

## `internal void PTagReset()`

Puts the panel back on the workspace open now.
No tag is chosen, nothing is selected, the editor is closed, and the tag catalog is re-read.
A different workspace has its own tags.
So the tag this panel stood on may not exist in the one now open.

## `internal bool PTagChangeCheck()`

Whether the editor holds modifications that have not been stored.
That is what the window asks before the workspace changes or the program closes.

## `internal void PTagClose()`

Stops the panel: the editor is shut down and the reader releases its playback.

## Inline notes

### `private PWindow _pTagHost = null!;`

The window this panel sits in.
It is who reports a read that failed.
It also asks the question put before unsaved work would be lost.

### `PDisplay.PDisplayAttach(host, engine);`

The reader is the same control the list panel mounts.
The window comes with it, because an incoming row opens the entry it names.
It is given the workspace because it fetches the flag of the language it shows.

### `PEditor.PEditorAttach(host, engine, null);`

The editor opens on no entry: this panel puts it on one when the reader asks to write.

### `PEditor.PEditorStoreDispatcher = PMembershipEntryUpdate;`

A store may have changed the tags the entry carries and the headword the membership row lists.
So the catalog and the entries under it are read again from what was written.
An entry can leave the chosen tag by being stored, and the panel must show that it did.

### `PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;`

Discarding here is not emptying a form.
The entry stays selected and comes back as it is stored.
