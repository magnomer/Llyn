# LTenureHerald.cs
Hash: `8aa200e2ae6fc5db`

## `internal sealed class LTenureHerald`

The tenure's voice toward the shell: the observer roster and the kept draft, which share one bulletin handler.
A draft notice clears the kept draft and reaches the roster through the same handler, so both live here.
It shares the tenure's gate and never owns one, so the lock order stays the turn, then the gate.
The tenure builds it last and keeps the turn, the ended check and the commit.

## `private readonly LBulletinRoster _lTenureHeraldRoster = new(null);`

The observers the shell attached to this tenure, each with the subject and draft identity it asked for.
The roster is read and emptied under the gate, and its callbacks run outside it.

## `private int _lTenureHeraldPreparing;`

How many preparations are running now.
While it is above zero, the handler drops the held draft's own draft notices.

## `private LDraft? _lTenureHeraldKept;`

The draft the last Mention find read, reused until a draft bulletin or a preparation clears it.

## `private int _lTenureHeraldRound;`

Counts the clears of the kept draft, so a read that raced a clear is answered but not kept.

## `internal LTenureHerald(LEngineHearth hearth, LDraftFacade draft, long id, object gate, LTenureQueue queue)`

Stores the hearth, the draft facade, the draft id, the tenure's gate and its queue.
The queue says whether the tenure still lives, so an ended tenure delivers nothing.
It subscribes its own bulletin handler through the hearth here, so the engine names no herald member to reach it.

## `internal void LTenureObserverInsert(LSubject subject, Action<LBulletin> observer, long? id)`

The one place an observer joins the roster, under the gate and only while the tenure lives.
A null id receives every notice of the subject.

## `private void LTenureBulletinHandle(LBulletin bulletin)`

The tenure's own engine subscription, which forwards a notice to the roster.
A draft notice first clears the kept draft, since the draft it holds may be stale.
An ended tenure forwards nothing.
During a preparation, the held draft's own draft notices are dropped.
The roster is read under the gate and dispatched outside it.

## `internal void LTenurePrepareStart()`

Counts one more running preparation under the gate.
The tenure calls it once its ended check passed, while it holds the turn.

## `internal void LTenurePrepareFinish()`

Counts one preparation done and drops the kept draft, under one hold of the gate.
So a Mention find never trusts a read taken mid-preparation.

## `internal LDraft? LTenureKeptRead()`

The held draft as the engine stored it, for the Mention find of `LQuillMention`.
The draft is kept until the next draft bulletin or prepare, so command checks read the file once per change.
A read that raced a bulletin is answered but not kept.
A stale draft after a workspace switch is refused, and the read then answers none.

## `private void LTenureKeptClear()`

Drops the kept draft and advances the round, so a read already under way is not kept.
It runs under the gate, since bulletins can arrive off the veneer's thread.

## `internal void LTenureObserverClear()`

Detaches the herald's handler through the hearth and empties the observer roster.
The tenure's cancel and successful finish call it, so nothing is delivered after the tenure ends.
