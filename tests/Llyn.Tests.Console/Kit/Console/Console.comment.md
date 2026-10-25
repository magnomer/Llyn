# Console

Tests of the console run loop, mirroring `src/Llyn.UIDemeanor/Kit/Console/`.

## `TLoop`

Three queued items run before the batch, so the batch runs once.
The batch completes the task the run waits for, so the test ends without a timeout.
A fault in a queued item goes to the handler the run was given, and not to the loop's own.
The key tests use a fake port that hands its keys over as one burst.
A burst of typed characters, an Enter and more characters reports two edits.
Enter is seen once, with the text typed before it.
A typed run followed by Enter and Tab keeps the order.
Each of those keys sees the text of the run.
A run that waits on a task holding two faults hands each fault to the handler the run was given.

## `TLoopNested`

Covers the loop with nested runs and the keys that reach them, each on a thread with a time limit.
An outer run that ends while an inner run hangs also ends the inner run, which then returns.
Keys that arrive while a settle is open still reach the observer in reader order.
A run that ends while a settle is open keeps the queued keys, and the next run receives them.
A nested run opened by a key receives the keys queued behind it.
A settle that opens a nested run for a modal box does the same.
The key after the box sees it closed.
A fault during a settle goes to the handler of the run, and the keys after it still arrive.
A fault during a settle that a dialog run waits through goes to the handler of that dialog run.
The loop's own handler, the outer run and a later run hear nothing of it.

## Stamp

Commit: `cb8ab7e8feb4`
- `TLoop.cs` `8a4bce21c0bfb036`
- `TLoopNested.cs` `6482cfdf62b7e3aa`
