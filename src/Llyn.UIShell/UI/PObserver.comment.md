# PObserver.cs

## `internal sealed class PObserver : LObserver`

The shell's end of the engine's stored-data announcement.
A surface hands over what to run and never implements the contract itself.
So a panel keeps its bulletin response private rather than making it a public member.

It also moves the call to the surface's own thread.
An engine call made off the UI thread announces on the thread that made it.
An import runs on a worker, so its announcement would otherwise reach a panel where it cannot touch a control.

## `internal PObserver(DispatcherObject surface, Action<LBulletin> target)`

Watches for `surface`, running `target` on the thread that surface belongs to.

## `internal PObserver(Action<LBulletin> target)`

Watches for something that owns no controls, running `target` on the announcing thread.

## `public void LObserverBulletinHandle(LBulletin bulletin)`

Runs the response now when the call already stands on the right thread.
Otherwise it queues the response there and returns, so the engine is never held for a redraw.
