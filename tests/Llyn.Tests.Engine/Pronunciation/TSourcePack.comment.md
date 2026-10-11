# TSourcePack.cs
Hash: `f8ba71575389e4b5`

## `public sealed class TSourcePack`

Covers real language packs reading IPA from Wiktionary's REST rendering.
The reader reads only the first page that answers, and the REST rendering answers first.
So a pack pattern that misses on that rendering never reaches the article page.
The REST rendering writes `data-mw-wikitext` before `id` on each section heading.
A heading anchor written as `<h2 id=` therefore missed every REST page.

## Inline notes

### `private const string TSourcePackKorean =`

The Korean section of the REST page for 사람, cut to its heading and IPA line.
The IPA span holds an empty category span, which the span reader skips.

### `private const string TSourcePackSpanish =`

The Spanish section of the REST page for agua, cut to its heading and IPA line.
The phonemic reading comes first and the phonetic one in brackets follows it.
