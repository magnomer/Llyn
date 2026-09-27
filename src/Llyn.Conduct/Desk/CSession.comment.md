# CSession.cs

## `public sealed class CSession`

One side panel's draft session over its desk, shared by the repertoire, the corpus and the guild.
It starts, finishes, saves, undoes and redoes the desk, and defers to the entry editor while it shows.
Each variant of a panel enters as a constructor parameter or a seam, never as a second copy.
The entry editor stays in Deportment until job06-10, so it enters through seams and its desk.

## `internal CSession(CDesk desk, IReadOnlyList<Func<bool>> pending, CDesk? editor, Func<bool> shownSeam, Func<bool, bool> finishSeam, Func<bool> readySeam, Action<long> storedSeam)`

The pending checks are asked in order whether a panel holds unsaved changes.
The editor desk takes over while the shown seam answers true, and the guild hands none.
The finish seam runs the editor's own finish while it shows.
A start opens the desk, which knows its own origin and subject or its vista.
The ready seam may refuse a store, and the stored seam shows what a store kept.

## `public event Action? CSessionHeld`

Raised after a start or a cancel, so the owner announces what the desk now holds.

## `public event Action? CSessionChanged`

Raised when the desk or the editor changes state.

## `public event Action<string, Exception>? CSessionFailed`

Raised with the desk scope's `HoldFailed` key and the exception when an undo or redo of the desk throws.

## `private CDesk? CSessionEditorRead()`

The editor desk while it shows, else nothing, so each step picks the editor or the desk.

## `public void CSessionStart(long? id)`

Starts the desk on a stored record or on nothing, then raises `CSessionHeld`.

## `public void CSessionCancel()`

Cancels the desk, then raises `CSessionHeld`.

## `public bool CSessionFinish(bool store)`

Finishes the editor while it shows, else the desk, showing a stored record again.
A store the ready seam refuses leaves the desk open and answers false.

## `public bool CSessionClose(bool store)`

Finishes like `CSessionFinish` but leaves a stored record unshown, because a row click shows its own row next.

## `public bool CSessionSave()`

Finishes the editor with a store while it shows and holds changes, else finishes the desk the same way.
The ready seam is asked first, so a blank guild name is refused even unchanged.

## `private bool CSessionReadyCheck(bool store)`

Asks the ready seam only for a store.

## `private void CSessionStoredShow(long id)`

Hands the stored record to the stored seam after the desk kept it.

## `public (bool CDeskBackward, bool CDeskForward) CSessionChronicleRead()`

Reads undo and redo from the editor's desk while it shows, else from the desk.

## `public void CSessionUndo()`

Steps the editor back while it shows, else the desk.

## `public void CSessionRedo()`

Steps the editor forward while it shows, else the desk.

## `private void CSessionChronicleRun(Action step)`

Runs a desk step and raises `CSessionFailed` with the desk scope's hold key when it throws.

## `public bool CSessionChangeCheck()`

True when any panel holds unsaved changes.

## `private void CSessionStateUpdate()`

Relays a desk or editor state change as `CSessionChanged`.
