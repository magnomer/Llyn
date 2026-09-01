# LSourceFactory.cs

## `public static class LSourceFactory`

Builds the live `LSource` set for a language pack. Every ordinary source becomes a data-driven `LSourceGeneric`; this is the seam where a future source needing logic a language pack cannot express would be wired to a hand-written handler instead. The orchestrators receive the finished set and stay language-agnostic.
