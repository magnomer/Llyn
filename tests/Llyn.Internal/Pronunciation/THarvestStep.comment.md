# THarvestStep.cs

## `public sealed class THarvestStep`

Covers the two step relays that turn a streamed contract into records handed to a delegate.
A listener relay over a list receives one source step, one recording per reading, and one end, in that order.
The source step names the source and its position, the end names nothing, and only a recording step carries one.
A receiver relay over a list receives the same shape with candidates.
The respelling wrapper over a relay still respells, so the candidate step carries the recast text.
