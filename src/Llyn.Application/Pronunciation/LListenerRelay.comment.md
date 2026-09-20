# LListenerRelay.cs

## `public sealed class LListenerRelay : LListener`

Turns the three listener calls into a stream of harvest steps handed to one delegate.
The harvest keeps its contract, and the caller keeps a delegate that crosses any depth as data.
So the shell never implements the contract, and the engine names no shell type to reach it.
Each call becomes exactly one step, on the thread that made the call.

## `public LListenerRelay(Action<LHarvestStep> sink)`

The sink is required, since a relay with nowhere to send is a caller mistake.

## `public void LListenerSourceStart(string source, int order)`

A source step naming the source and its position, with no recording.

## `public void LListenerRecordingAdd(LRecording recording)`

A recording step, with the source and position read off the recording itself.

## `public void LListenerFinish()`

The end step, naming no source and carrying order zero.
