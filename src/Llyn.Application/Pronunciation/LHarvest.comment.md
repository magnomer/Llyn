# LHarvest.cs

## `public sealed class LHarvest`

Fans an audio request out to every source it was given at once.
It runs the whole set, because the caller hands it the recording sources alone.
Each recording carries the position its source holds in that set, so a slow source keeps its declared place.
Streams each downloadable recording back to the listener as it arrives.
The download counterpart to `LLookup`.
One slow or failing source never blocks or fails the others.
Every source produces exactly one recording, whether it answered, had nothing, or was never reached.
Each source runs under its own minute-long deadline, exactly as a lookup source does.
The discovery reports complete once all sources have finished.
It also returns the whole set it streamed, in source order, exactly as `LLookup` does.
The sources are supplied ready-built and language-agnostic, so this orchestrator knows nothing about any particular source or language.

## `private static Task<LAnswer> LHarvestAnswerRead(LSource source, string word, CancellationToken cancellation)`

The recording counterpart of `LLookupAnswerRead`.
