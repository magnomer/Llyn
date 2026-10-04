# LMarkupEtymology.cs
Hash: `c81d407310e96677`

## `public sealed record LMarkupEtymology(string LMarkupEtymologyText = "", IReadOnlyList<LMarkupMention>? LMarkupEtymologyMention = null)`

The narrative shape of an entry's etymology as a markup file carries it.
Each span inside the text names a source entry by headword and language.

**Parameters**

- `LMarkupEtymologyText` — The explanation as written, untrimmed.
- `LMarkupEtymologyMention` — The spans of that text which name an entry.
