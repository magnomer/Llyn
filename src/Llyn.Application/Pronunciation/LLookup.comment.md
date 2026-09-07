# LLookup.cs

## `public sealed class LLookup : LSeeker`

Fans a pronunciation request out to every source it was given at once.
It runs the whole set, because the caller hands it the transcription sources alone.
Each candidate carries the position its source holds in that set, so a slow source keeps its declared place.
Streams each result back to the receiver as it arrives.
One slow or failing source never blocks or fails the others.
The lookup reports complete once all sources have finished.
The sources are supplied ready-built and language-agnostic (see `LSource`).
This orchestrator knows nothing about any particular source or language.

## Inline notes

### `return;`

A failed source yields no candidate.
The lookup still completes with the others.
