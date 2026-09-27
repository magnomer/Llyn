# QSession.cs

## `public sealed class QSession`

One side panel's draft session over its desk, shared by the repertoire, the corpus and the guild.
It starts, finishes, saves, undoes and redoes the desk, and defers to the entry editor while it shows.
Each variant of a panel enters as a constructor parameter or a seam, never as a second copy.
It is medium-free, so it takes `Q` until job44 sinks it into Conduct.

## `internal QSession(LDesk desk, IReadOnlyList<LPanel> panels, LEditor? editor, LPanel? scribe, string hold, Action<long?> startSeam, Func<bool> readySeam, Action<long> storedSeam)`

The panels are asked in order whether they hold unsaved changes.
The editor takes over while the scribe panel edits, and the guild hands neither.
The hold key names a failed undo or redo.
The start seam opens the desk, by subject on the repertoire and the corpus and by vista on the guild.
The ready seam may refuse a store, and the stored seam shows what a store kept.

## `public event Action? QSessionHeld`

Raised after a start or a cancel, so the owner announces what the desk now holds.

## `public event Action? QSessionChanged`

Raised when the desk or the editor changes state.

## `public event Action<string, Exception>? QSessionFailed`

Raised with the hold key and the exception when an undo or redo of the desk throws.

## `private bool QSessionEditorShown`

True while the scribe panel edits, and false without a scribe.

## `private LEditor? QSessionEditorRead()`

The editor while it shows, else nothing, so each step picks the editor or the desk.

## `public void QSessionStart(long? id)`

Starts the desk on a stored record or on nothing, then raises `QSessionHeld`.

## `public void QSessionCancel()`

Cancels the desk, then raises `QSessionHeld`.

## `public bool QSessionFinish(bool store)`

Finishes the editor while it shows, else the desk, showing a stored record again.
A store the ready seam refuses leaves the desk open and answers false.

## `public bool QSessionClose(bool store)`

Finishes like `QSessionFinish` but leaves a stored record unshown, because a row click shows its own row next.

## `public bool QSessionSave()`

Saves the editor while it shows, else finishes the desk when it holds changes.
The ready seam is asked first, so a blank guild name is refused even unchanged.

## `private bool QSessionReadyCheck(bool store)`

Asks the ready seam only for a store.

## `private void QSessionStoredShow(long id)`

Hands the stored record to the stored seam after the desk kept it.

## `public (bool LDeskBackward, bool LDeskForward) QSessionChronicleRead()`

Reads undo and redo from the editor's desk while it shows, else from the desk.

## `public void QSessionUndo()`

Steps the editor back while it shows, else the desk.

## `public void QSessionRedo()`

Steps the editor forward while it shows, else the desk.

## `private void QSessionChronicleRun(Action step)`

Runs a desk step and raises `QSessionFailed` with the hold key when it throws.

## `public bool QSessionChangeCheck()`

True when any panel holds unsaved changes.

## `private void QSessionStateUpdate()`

Relays a desk or editor state change as `QSessionChanged`.
