# LTenureQueue.cs
Hash: `b6e8abb53a3b9e86`

## `internal sealed class LTenureQueue`

The request pipeline of one tenure, holding the locks, the halt, the end and the apply.
It also answers whether the tenure still takes requests, so every check reads one owner.
It shares the tenure's gate and turn, so a check and the change it allows share one hold.
The deferred requests and the quiet before their flush belong to `LChronicleClerk`.
The queue hands each deferral to the clerk and flushes when the clerk's wait runs out.

## `private readonly LEngine _lTenureQueueEngine;`

The engine every request is applied to.

## `private readonly LChronicleClerk _lTenureQueueChronicle;`

The clerk keeping the deferred requests and timing their flush.
Read once at construction, so a workspace switch cannot strand requests in a clerk the queue no longer reads.

## `private readonly long _lTenureQueueId;`

The draft id, carried by the draft bulletin a refusal raises and handed to undo and redo.
It also names this tenure's deferred requests at the clerk.

## `private readonly object _lTenureQueueGate;`

The tenure's gate, guarding every call to the clerk, the fault and the ended flag.

## `private readonly object _lTenureQueueTurn;`

The tenure's turn, taken by the timer's flush and by undo and redo.

## `private readonly Action _lTenureQueueObserver;`

The tenure's state raise, called after every apply, restore and halt.

## `private Exception? _lTenureQueueFault;`

The failure that halted the tenure, or null while it runs.
Written once, under the gate.

## `internal LTenureQueue(LEngine engine, long id, object gate, object turn, Action observer)`

Made by the tenure alone, handed its locks and its state raise.

## `internal Exception? LTenureQueueFault`

The failure that halted the tenure, read under the gate.
It takes the gate itself, so a caller holding it or not reads the same.
The finish throws the answer outside the gate, so no exception filter runs while the gate is held.

## `internal bool LTenureQueueEnded { get; private set; }`

Whether the draft has been committed or cancelled, after which the tenure is inert.
Read under the gate.

## `internal bool LTenureQueueLive`

Whether the tenure neither ended nor halted, so a request may still apply.

## `internal async Task LTenureQueueStart(LRequest request, int delay)`

Hands the request to the clerk and, once the clerk's wait runs out, flushes under the turn.
The clerk files the request before the first await, so it is queued when the call returns.
The task is not awaited, because the keystroke that started it must return at once.
A delay of zero files the request and starts no wait, leaving the flush to the caller.
A cancelled or replaced wait answers null and flushes nothing.
The flush resumes on a pool thread, and the engine's observers marshal to their own threads.
Called under the gate.

## `internal bool LTenureQueueClose()`

Drops the deferred requests and their wait at the clerk, and marks the tenure ended.
Answers false when it had already ended, so a cancel runs its discard only once.
Called under the gate.

## `internal void LTenureQueueDispatch(CancellationTokenSource? pending)`

Takes the deferred requests from the clerk under the gate and applies what it held.
Given the wait it resumes from, it applies nothing when a newer wait has replaced that one.
The take and the check share one hold of the gate, so no deferral slips in between them.
Called under the turn.

## `internal void LTenureQueueApply(IReadOnlyList<LRequest> requests)`

Applies the requests in order and announces the state afterwards.
A refused request is skipped and the rest still apply, and one draft bulletin follows so the panel refills.
A refusal saying the draft is gone is a lost hold, not a refused edit, and halts like any failure.
The first other failure halts the tenure and the rest are dropped.
Called under the turn.

## `internal LDraft? LTenureQueueRestore(Func<long, LDraft?> step)`

The one path undo and redo share, writing what was waiting before it steps.
A halted or ended tenure steps nowhere.

## `internal void LTenureQueueSuspend(Exception exception)`

Halts the tenure once, keeping the failure for the finish to rethrow.
The deferred requests and their wait are dropped at the clerk, since nothing further applies.
