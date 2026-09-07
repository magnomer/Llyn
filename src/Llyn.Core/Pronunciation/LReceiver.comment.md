# LReceiver.cs

## `public interface LReceiver`

Subscriber contract for a pronunciation lookup.
The requester implements this to receive streamed results.
Each source reports when it starts, and each reports exactly one candidate when it is done.
The lookup reports once when all sources have finished.
So a menu can stand a row per source from the first moment and resolve each in place.
Callbacks may arrive on background threads.
An implementation that touches UI is responsible for marshalling.

## `void LReceiverSourceStart(string source, int order);`

A source has begun searching.
`source` is the source's name and `order` is its position in the language pack's list.
The position comes with the start, so a row can be placed before any answer exists to place it by.

## `void LReceiverCandidateAdd(LCandidate candidate);`

A source has finished and this is what it had.
It arrives whether the source found a transcription, had none, or could not be reached.

## `void LReceiverLookupFinish();`

Every source has finished.
No further callbacks follow.
