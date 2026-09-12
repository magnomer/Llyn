# TLookup.cs

## `public sealed class TLookup`

Covers the lookup fanning one source's readings out into candidates.
Every reading of a source becomes one candidate sharing the source's order and keeping its variety.
An empty answer still yields one candidate without a phonetic, so the shell can say why.
The receiver hears the finish exactly once, and never when the lookup was cancelled.
