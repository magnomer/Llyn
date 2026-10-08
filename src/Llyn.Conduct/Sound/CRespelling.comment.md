# CRespelling.cs
Hash: `cfc0ee8f72ea8f63`

## `public static class CRespelling`

The respelling rules of the reflex rows: which form of a reading a driver prints, and what stands around it.
It holds no state, so it is static and the atelier builds nothing for it.
A found reading's mark and text come ready from the errand's search instead.

## `internal static IReadOnlyList<CReflex> LRespellingReflexScan(LReflexPort port, string language, IReadOnlyList<CReflexDraft> reflexes)`

The reflex rows of an entry in `language`, in order, each ready to show.
One engine read answers every row's switch, phonemic mark and fold.
Each row is marked when it leads its run of one language.
The editor and the reading view's sound area both read their rows here, so the rule has one owner.

## `private static CReflex LRespellingReflexRead(CReflexDraft reflex, LReflexGuise guise, bool lead)`

One reflex row under the mark its guise picks.
A phonemic language stands between slashes whatever the switch shows, and any other reading stands bare.
The text is resolved by `LRespellingResolve`.

## `internal static string LRespellingResolve(CRespellingMark mark, string phonetic, string? respelling)`

The respelling while the mark shows it and it is filled, and the phonetic otherwise.
