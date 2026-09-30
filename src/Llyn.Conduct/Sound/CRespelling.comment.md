# CRespelling.cs

## `public sealed class CRespelling`

The respelling gates: which form of a reading a driver prints, and what stands around it.
Every notation answer is read here, so no driver chooses a bracket or a slash.
It stands on the atelier's phonology port, and `CAtelier` builds the one instance every driver shares.

## `internal CRespelling(CAtelier atelier)`

Only the atelier builds it, so each session has one.

## `public CRespellingMark CRespellingMarkRead(string language, bool schemed)`

The mark a transcription candidate prints under.
A schemed search prints the phonetic bare, whatever the switch shows.
Otherwise the engine answers the switch and both brackets, so the bracket rule has one owner below.

## `internal static IReadOnlyList<CReflex> LRespellingReflexScan(`

The reflex rows of an entry in `language`, in order, each ready to show.
One engine read answers every row's switch, phonemic mark and fold.
Each row is marked when it leads its run of one language.
The editor and the reading view's sound area both read their rows here, so the rule has one owner.

## `private static CReflex LRespellingReflexRead(CReflexDraft reflex, LReflexGuise guise, bool lead)`

One reflex row under the mark its guise picks.
A phonemic language stands between slashes whatever the switch shows, and any other reading stands bare.
The text is resolved by `CRespellingResolve`.

## `public static string CRespellingResolve(CRespellingMark mark, string phonetic, string? respelling)`

The respelling while the mark shows it and it is filled, and the phonetic otherwise.
