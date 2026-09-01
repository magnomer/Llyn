# PList.xaml.cs

## `public partial class PList : UserControl`

The list panel as a control: what it is made of, and when it starts and stops. Browsing itself — the search, the ordering, the index, the read-only display and the editor beside it — lives in the file beside this one.

## `internal void PListAttach(PWindow host, LEngine engine)`

Puts the panel to work on `engine`, the workspace the window opened. Nothing is read yet: the panel fills itself the first time it is shown, so a session that never opens the List tab never queries the database.

## `internal void PListReset()`

Puts the panel back on the workspace open now: nothing is selected, the editor is closed, and the index is re-read. A different workspace has its own database, so what the panel was showing came from one that is no longer open.

## `internal bool PListChangeCheck()`

Whether the editor holds modifications that have not been stored: what the window asks before the workspace changes or the program closes.

## `internal void PListClose()`

Stops the panel: the editor is shut down and this panel's own playback is released.

## Inline notes

### `private PWindow _pListHost = null!;`

The window this panel sits in, which is who reports a load that failed and asks the question put before unsaved work would be lost.

### `PEditor.PEditorAttach(host, engine, null);`

The editor opens on no entry: this panel puts it on one when the reader asks to write.

### `PEditor.PEditorStoreDispatcher = PIndexEntryUpdate;`

A store may have changed the headword the index lists and the text the display shows, so both are read again from what was written rather than left as they were.

### `PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;`

Discarding here is not emptying a form: the entry stays selected and comes back as it is stored, which is what there is to fall back to.
