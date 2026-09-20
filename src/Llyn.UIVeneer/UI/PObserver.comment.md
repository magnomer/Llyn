# PObserver.cs

## `internal static class PObserver`

The shell's end of the engine's announcements and streamed steps.
A surface hands over what to run and never implements a contract itself.
So a panel keeps its bulletin response private rather than making it a public member.
The engine takes a delegate, so the wrapped delegate is what a surface attaches and later detaches.

It also moves the call to the surface's own thread.
An engine call made off the UI thread announces on the thread that made it.
An import runs on a worker, so its announcement would otherwise reach a panel where it cannot touch a control.
A lookup step arrives from a source's worker for the same reason.

## `internal static Action<LBulletin> PObserverCreate(DispatcherObject surface, Action<LBulletin> target)`

A bulletin delegate for `surface`, running `target` on the thread that surface belongs to.

## `internal static Action<LBulletin> PObserverCreate(DispatcherObject surface, Action target)`

A bulletin delegate for `surface` with a response that needs nothing from the bulletin.
A list that re-reads itself whole on a subject is attached this way, with no wrapper method of its own.

## `internal static Action<PStep> PObserverCreate<PStep>(DispatcherObject surface, Action<PStep> target)`

A delegate over any record for `surface`, running `target` on the thread that surface belongs to.
The bulletin overloads are this with a bulletin, and the clip and notation menus use it with their step records.
Runs the response now when the call already stands on the right thread.
Otherwise it queues the response there and returns, so the engine is never held for a redraw.
