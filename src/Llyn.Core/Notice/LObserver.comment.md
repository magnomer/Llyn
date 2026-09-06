# LObserver.cs

## `public interface LObserver`

Subscriber contract for stored data.
A shell surface implements this to learn that a record changed.
It is the third seam of the same shape as `LReceiver` and `LListener`.
Those two stream one answer to the caller that asked.
This one announces a change to every subscriber, including the one that made it.
Callbacks arrive on the thread that made the change.

## `void LObserverBulletinHandle(LBulletin bulletin);`

A stored record changed.
The subscriber re-reads whatever it holds of that kind.
