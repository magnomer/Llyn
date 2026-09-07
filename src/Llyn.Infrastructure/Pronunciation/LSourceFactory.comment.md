# LSourceFactory.cs

## `public static class LSourceFactory`

Builds a live `LSource` set from the source definitions it is handed.
It is called once per list, so a transcription set and a recording set never mix.
Every ordinary source becomes a data-driven `LSourceGeneric`.
This is the seam where a future source needing logic a pack cannot express would be wired.
Such a source would go to a hand-written handler instead.
The orchestrators receive the finished set and stay language-agnostic.
