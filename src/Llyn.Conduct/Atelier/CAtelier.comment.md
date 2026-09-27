# CAtelier.cs

## `public sealed class CAtelier : IDisposable`

Conduct's root: the working session over one workspace, which Host builds and hands to a driver.
It holds the six engine ports and the session's posture.
Its gates are the medium-free actions no single panel owns.
Those are workspace change, leftover sweep, bulletins, vista start, volume, split and the open view.
GUI-only state such as window geometry and panel widths never reaches it.

## `internal CAtelier(`

Takes the posture and the six ports Host builds over one engine.
It is internal, so no driver can build a root of its own.

## `internal LDraftPort CAtelierDraftPort { get; }`

A transitional handle for the deportments not yet moved into Conduct.
The five other port handles below it stand for the same reason.
job14-30 deletes all six.

## `internal LVista CAtelierVistaStart(string tab, CSubject? subject, CCatalogOrder fallback, bool blank = false)`

Starts the tab's vista on its stored order, filter and split.
The driver names the subject and order in Conduct's enums, so it never names a Core one.
It is internal, since the vista it answers is an engine handle.

## `public bool CAtelierModeMatch(string? mode)`

Whether the tab named is the one stored as standing open.

## `public void CAtelierModeSave(string mode)`

Stores the name of the tab standing open.

## `public bool CAtelierSplitRead()`

Whether the open tab shows its editor rather than its read area.

## `public double CAtelierVolumeRead()`

The session's one audio level, between zero and one.

## `public void CAtelierVolumeSet(double volume, bool settled)`

Sets the one audio level and hands it to the player.
The level is written only when `settled`, so a drag costs no write per step.
A driver passes `settled` once the gesture ends, and a key press is settled at once.

## `public void CAtelierLeftoverSweep()`

Drops the draft files no editor holds any more, and the recordings no entry keeps.
A driver calls it once the session opens.

## `public void CAtelierWorkspaceChange(string path)`

Moves the session onto the workspace at `path`.
The engine records the folder only once the move succeeds.

## `public void Dispose()`

Sweeps the leftover drafts once more, then releases the posture, which lets go of every vista it watched.
Sweeping on the way out as well as on the way in bounds what a long session leaves behind.

## `public Action CAtelierObserverAttach(Action<CBulletin> observer)`

Hands every engine bulletin to `observer` as a Conduct bulletin, and answers the detach.

## `public Action CAtelierObserverAttach(CSubject subject, Action<CBulletin> observer)`

Hands `observer` only the bulletins about `subject`, and answers the detach.
The engine subject is read into Conduct's enum, so the compare stays between Conduct values.
