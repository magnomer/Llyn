# TCatalogPronunciation.cs
Hash: `54805b3d15bbc78c`

## `public sealed class TCatalogPronunciation`

Covers the phonology catalog, where every row carries the pronunciation stored for its entry.
It covers headword order and its reverse, which this panel shares with the library.
It covers the sound and headword orderings, which leave out an entry with nothing stored.
It covers the pending ordering, which lists only such entries.
It covers the respelling setting, under which a row carries its respelling instead of its IPA.
It covers the row carrying its sound, because the panel shows one and orders by it.
It covers the bracketed text, which is an empty pair of brackets where nothing is stored.
It covers equal sounds, which go by language, then headword, then entry id.
It covers equal headwords, which go by language and then entry id.
So rows stored out of order never surface in storage order.

