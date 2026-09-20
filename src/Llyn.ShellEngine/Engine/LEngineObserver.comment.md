# LEngineObserver.cs

## `public sealed partial class LEngine`

The announcement side of the engine boundary.
The rest of the engine answers the call that asked.
This part tells every subscriber that stored data changed, including the one whose call changed it.

A subscriber therefore never learns of a change by being named and told.
Before this, the shell wired a panel's method into a sibling panel, one method per pair.
A tenth panel meant a tenth wire, and a panel nobody wired stayed stale until it was shown again.

## `public void LEngineObserverAttach(Action<LBulletin> observer)`

Subscribes `observer` to every announcement from here on.
A subscriber is a delegate, so a bulletin crosses any depth as data and no ring implements a contract.
A delegate already attached is not added twice, so a surface re-attached keeps one voice.
Delegate equality is target plus method, so a method group attached twice is one subscriber.

## `public void LEngineObserverDetach(Action<LBulletin> observer)`

Stops announcing to `observer`.
A surface that is closing detaches the same delegate it attached, so a dead control is never called.

## `internal void LEngineBulletinRaise(LSubject subject, long id)`

Announces one change to every subscriber.
The list is copied under the gate and the calls are made outside it.
A subscriber re-reads through the engine, and the gate is reentrant, so it would hold either way.
Copying keeps a subscriber that detaches during the announcement from disturbing the walk.
Internal rather than private, because a tenure raises its own state bulletin through it.

Announcements are made where a stored record is finished, not where each step of it is written.
An entry save, create, or update announces nothing on its own.
Those are the steps a commit or an import is made of, and both announce once when they are done.
So importing a thousand entries costs the shell one refresh rather than a thousand.
