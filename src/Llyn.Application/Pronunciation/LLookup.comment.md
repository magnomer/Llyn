# LLookup.cs

## `public sealed class LLookup : LSeeker`

Fans a pronunciation request out to every configured pronunciation source concurrently and streams each result back to the receiver as it arrives. One slow or failing source never blocks or fails the others; the lookup reports complete once all sources have finished. The sources are supplied ready-built and language-agnostic (see `LSource`); this orchestrator knows nothing about any particular source or language.

## Inline notes

### `return;`

A failed source yields no candidate; the lookup still completes with the others.
