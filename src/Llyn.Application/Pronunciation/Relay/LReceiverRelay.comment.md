# LReceiverRelay.cs

## `public sealed class LReceiverRelay : LReceiver`

Turns the three receiver calls into a stream of lookup steps handed to one delegate.
The seeker keeps its contract, and the caller keeps a delegate that crosses any depth as data.
So the shell never implements the contract, and the engine names no shell type to reach it.
Each call becomes exactly one step, on the thread that made the call.
A respelling wrapper sits in front of it like any receiver, so cached and live lookups respell alike.

## `public LReceiverRelay(Action<LLookupStep> sink)`

The sink is required, since a relay with nowhere to send is a caller mistake.

## `public void LReceiverSourceStart(string source, int order)`

A source step naming the source and its position, with no candidate.

## `public void LReceiverCandidateAdd(LCandidate candidate)`

A candidate step, with the source and position read off the candidate itself.

## `public void LReceiverLookupFinish()`

The end step, naming no source and carrying order zero.
