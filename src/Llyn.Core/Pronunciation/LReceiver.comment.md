# LReceiver.cs

## `public interface LReceiver`

Subscriber contract for a pronunciation lookup.
The requester implements this to receive streamed results.
Each source reports when it starts.
Every candidate is delivered as it arrives.
The lookup reports once when all sources have finished.
Callbacks may arrive on background threads.
An implementation that touches UI is responsible for marshalling.

## `void LReceiverSourceStart(string source);`

A source has begun searching.
`source` is the source's name.

## `void LReceiverCandidateAdd(LCandidate candidate);`

A candidate has arrived from a source.

## `void LReceiverLookupFinish();`

Every source has finished.
No further callbacks follow.
