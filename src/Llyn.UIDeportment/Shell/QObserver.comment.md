# QObserver.cs

## `public static class QObserver`

The shell's end of Conduct's announcements and streamed steps.
It names no engine record, so a caller names the record it attaches.
A surface hands over what to run and never implements a contract itself.
So a panel keeps its bulletin response private rather than making it a public member.
The engine takes a delegate, so the wrapped delegate is what a surface attaches and later detaches.

It also moves the call to the surface's own thread.
An engine call made off the UI thread announces on the thread that made it.
An import runs on a worker, so its announcement would otherwise reach a panel where it cannot touch a control.
A lookup step arrives from a source's worker for the same reason.

## `public static Action<LStep> QObserverCreate<LStep>(DispatcherObject surface, Action target)`

A delegate over any record for `surface` with a response that needs nothing from the record.
A view attaches a sealed panel's notice this way, naming the Conduct notice instead of the engine's.
A list that re-reads itself whole on a subject is attached this way, with no wrapper method of its own.

## `public static Action<LStep> QObserverCreate<LStep>(DispatcherObject surface, Action<LStep> target)`

A delegate over any record for `surface`, running `target` on the thread that surface belongs to.
The overload above is this with the record dropped.
The clip and notation menus use it with their step records.
Runs the response now when the call already stands on the right thread.
Otherwise it queues the response there and returns, so the engine is never held for a redraw.

## `public static Action<LStep> QObserverCreate<LStep>(Action<LStep> target)`

The same marshalling onto the dispatcher of the thread that attaches.
A driver attaches on the UI thread, so it needs no surface to find that dispatcher.
