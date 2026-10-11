# CRespelling.cs
Hash: `ece0c93bdb9fa88a`

## `public static class CRespelling`

The reflex rows as a driver prints them, each mapped from its engine guise.
It holds no state, so it is static and the atelier builds nothing for it.
A found reading's mark and text come ready from the errand's search instead.

## `internal static IReadOnlyList<CReflex> LRespellingReflexScan(LReflexPort port, string language, IReadOnlyList<LReflexDraft> reflexes)`

The reflex rows of an entry in `language`, in order, each ready to show.
One engine read over the rows answers every row's switch, phonemic mark, fold and shown text.
The rows are then built by the overload below.
The editor and the reading view's sound area both read their rows here, so the rule has one owner.

## `internal static IReadOnlyList<CReflex> LRespellingReflexScan(IReadOnlyList<CReflexDraft> reflexes, IReadOnlyList<LReflexGuise> guises)`

The reflex rows, in order, each ready to show under the guise at its position.
Each row is marked when it leads its run of one language.
A series member, whose guises the engine already read, builds its rows here, so they match the entry page's rows.

## `private static CReflex LRespellingReflexRead(CReflexDraft reflex, LReflexGuise guise, bool lead)`

One reflex row under the mark its guise picks.
A phonemic language stands between slashes whatever the switch shows, and any other reading stands bare.
A blank region becomes null, so a driver binds the region tooltip as it stands.
The text is the guise's `LReflexGuiseShown`, mapped as the engine answers it, so no respelling rule lives here.
