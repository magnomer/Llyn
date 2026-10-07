# TCatalogPronunciation.cs
Hash: `c560e46bc2da8cba`

## `public sealed class TCatalogPronunciation`

Covers the phonology catalog, where every row carries the pronunciation stored for its entry.
It covers headword order and its reverse, which this panel shares with the library.
It covers the sound ordering, which puts an entry with nothing stored last.
It covers the pending ordering, which puts that same entry first.
It covers the row carrying its sound, because the panel shows one and orders by it.
It covers the bracketed text, which is an empty pair of brackets where nothing is stored.
It covers equal sounds, which go by language, then headword, then entry id.
It covers equal headwords, which go by language and then entry id.
So rows stored out of order never surface in storage order.

