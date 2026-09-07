# LSeeker.cs

## `public interface LSeeker`

Pronunciation lookup contract.
A requester starts a lookup and subscribes to its results through an `LReceiver`.
All lookup logic lives behind this seam, never in the UI.

## `Task<IReadOnlyList<LCandidate>> LSeekerStart(string word, LReceiver receiver, CancellationToken cancellation);`

Starts a lookup for `word`, streaming results to `receiver`.
The returned task completes when every source has finished (or the lookup is cancelled).
It carries the whole set of candidates that were streamed, ordered by their sources.
A caller that wants to keep the answer therefore need not collect the stream a second time.
A cancelled lookup returns nothing, because the task ends by throwing instead.
