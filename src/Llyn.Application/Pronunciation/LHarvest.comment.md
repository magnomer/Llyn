# LHarvest.cs

## `public sealed class LHarvest`

Fans an audio request out to every configured audio source concurrently and streams each downloadable recording back to the listener as it arrives. The download counterpart to `LLookup`: one slow or failing source never blocks or fails the others, and the discovery reports complete once all sources have finished. The sources are supplied ready-built and language-agnostic, so this orchestrator knows nothing about any particular source or language.

## Inline notes

### `return;`

A failed source yields no recording; the discovery still completes with the others.
