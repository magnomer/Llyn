# CSession.cs
Hash: `9ad66eecc6709ab4`

## `public sealed class CSession`

One side panel's draft session over its desk, shared by the repertoire, the corpus, the guild and the shelf.
It starts, finishes, saves, undoes and redoes the desk, and defers to the entry editor while it shows.
It also asks before leaving unsaved changes and closes the editor when the workspace closes.
Each variant of a panel enters as a constructor parameter or a seam, never as a second copy.

## `internal CSession(CDesk desk, IReadOnlyList<Func<bool>> pending, CEditor? editor, Func<bool> shownSeam, Func<bool, bool> finishSeam, Func<bool> readySeam, Action<long> storedSeam, CEnvoy envoy)`

The pending checks are asked in order whether a panel holds unsaved changes.
The editor's desk takes over while the shown seam answers true, and the guild hands no editor.
The finish seam runs the editor's own finish while it shows.
A start opens the desk, which knows its own origin and subject or its vista.
The ready seam may refuse a store, and the stored seam shows what a store kept.
The envoy asks the user before unsaved changes are left.

## `public event Action? CSessionHeld;`

Raised after a start or a cancel, so the owner announces what the desk now holds.

## `public event Action? CSessionChanged;`

Raised when the desk or the editor changes state.

## `private CDesk? CSessionEditorRead()`

The editor desk while it shows, else nothing, so each step picks the editor or the desk.

## `public void CSessionStart(long? id)`

Starts the desk on a stored record or on nothing, then raises `CSessionHeld`.

## `public void CSessionCancel()`

Cancels the desk, then raises `CSessionHeld`.

## `internal bool LSessionFinish(bool store)`

Finishes the editor while it shows, else the desk, showing a stored record again.
A store the ready seam refuses leaves the desk open and answers false.

## `public bool CSessionClose(bool store)`

Finishes like `LSessionFinish` but leaves a stored record unshown, because a row click shows its own row next.

## `public bool CSessionSave()`

Stores the editor while it shows and holds changes, else stores the desk while it is held and changed.
The ready seam is asked first for the desk, so a blank guild name is refused even unchanged.
A held desk with no changes stays open and answers true.

## `private bool CSessionReadyCheck(bool store)`

Asks the ready seam only for a store.

## `private void CSessionStoredShow(long id)`

Hands the stored record to the stored seam after the desk kept it.

## `public (bool CDeskBackward, bool CDeskForward) CSessionChronicleRead()`

Reads undo and redo from the editor's desk while it shows, else from the desk.

## `public void CSessionUndo()`

Steps the editor back while it shows, else the desk.
Each desk shows its own failed step, so the session catches nothing.

## `public void CSessionRedo()`

Steps the editor forward while it shows, else the desk.

## `internal bool LSessionLeaveConfirm(bool shown)`

Asks the user only when a panel holds unsaved changes, and answers whether leaving may go on.
A cancelled question answers false, and so does a store the ready seam or the tenure refuses.
A shown record is finished and shown again, while a record left unshown is closed for a row click.

## `internal void LSessionEditorClose()`

Closes the entry editor and cancels its playback when the workspace closes.
It leaves the session's own desk alone, and a session without an editor does nothing.

## `internal bool LSessionChangeCheck()`

True when any panel holds unsaved changes.

## `private void LSessionStateUpdate()`

Relays a desk or editor state change as `CSessionChanged`.
