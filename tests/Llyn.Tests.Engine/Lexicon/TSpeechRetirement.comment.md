# TSpeechRetirement.cs
Hash: `9c83a1639d8c4839`

## `public sealed class TSpeechRetirement`

Its helpers keep the `TSpeech` base, since a retirement is a fact of the speech store.
Covers the retirement of a part of speech a pack once published.
Most tests write a small fixture vocabulary under a language no pack ships, so the shipped packs never interfere.
It covers a link moved to the head, a duplicate link dropped, and links dropped.
It covers a unit set on linking entries.
It covers a second application that finds nothing to do.
It covers the shipped English pack read for its retired rows.
It covers an import that keeps a part the user added.
