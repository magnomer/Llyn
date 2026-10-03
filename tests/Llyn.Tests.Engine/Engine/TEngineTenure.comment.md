# TEngineTenure.cs
Hash: `ac07d8236f6f41a4`

## `public sealed class TEngineTenure`

The tenure's sequence, one fact per promise the panel relies on.
Each fact starts a tenure on a fresh workspace and drives it through the relays alone.
The delay seam is a long hold where the queue must be seen, and zero where writes land at once.
The commit of each subject lives in `TEngineTenureSubject`, and the starts from a given row in `TEngineTenureVista`.

## `internal const int TTenureHold = 600000;`

A wait no test outlives, so a deferred request stays queued until the fact flushes it by hand.
Every `TEngineTenure` sibling class waits through it too.

## `public void TenureDefer_SameKey_AppliesLast()`

Two deferrals of the same field leave only the second, and nothing is written until the flush.

## `public void TenureFinish_Unchanged_Cancels()`

Finishing a draft nobody changed stores nothing and drops the draft, answering null.

## `public void TenureDefer_ApplyThrows_MarksHalted()`

A request whose draft is gone halts the tenure.
After that the finish refuses and the undo steps nowhere.
The draft is cancelled behind the tenure's back, which is the refusal a lost workspace would raise.

## `public void TenureUndo_AfterDefer_PersistsFirst()`

An undo writes the deferred request first, so it steps back from what was on screen.
Redo returns to it.
The clock is advanced past the chronicle's merge window, so the two writes are two steps.

## `public void TenureChangeCheck_FreshEntry_AnswersUnchanged()`

A fresh blank entry reads unchanged and not storable.

## `public void TenureDefer_GlossTwice_OneChronicleStep()`

Two deferrals of one gloss text reach the chronicle as one step, so one undo clears the gloss.
Were each deferral written on its own, the undo would step back only to the first text.

## `public void TenureDefer_Refused_KeepsRunning()`

A request the engine refuses, here a collocation nested under a card, is dropped and the tenure runs on.
The headword deferred after it still applies, and a draft bulletin is raised so a panel refills from the draft.

## `public void TenureFinish_Discard_LeavesWaiting()`

Finishing without storing cancels before the queue is written, so the discarded text never reaches the draft.
No draft bulletin is raised, since nothing was applied.
