# TEngineTenure.cs

## `public sealed class TEngineTenure`

The tenure's sequence, one fact per promise the panel relies on.
Each fact starts an example tenure on a fresh workspace and drives it through the relays alone.
The delay seam is a long hold where the queue must be seen, and zero where writes land at once.

## `private const int TTenureHold = 600000;`

A wait no test outlives, so a deferred request stays queued until the fact flushes it by hand.

## `public void TenureDefer_SameKey_AppliesLast()`

Two deferrals of the same field leave only the second, and nothing is written until the flush.

## `public void TenureFinish_Unchanged_Cancels()`

Finishing a draft nobody changed stores nothing and drops the draft, answering null.

## `public void TenureFinish_Changed_CommitsExample()`

Finishing a changed example tenure stores the example, drops the draft and answers the stored id.

## `public void TenureDefer_ApplyThrows_MarksHalted()`

A request the engine refuses halts the tenure, after which the finish refuses and the undo steps nowhere.
The draft is cancelled behind the tenure's back, which is the refusal a lost workspace would raise.

## `public void TenureUndo_AfterDefer_PersistsFirst()`

An undo writes the deferred request first, so it steps back to what was on screen, and redo returns.
The clock is advanced past the chronicle's merge window, so the two writes are two steps.
