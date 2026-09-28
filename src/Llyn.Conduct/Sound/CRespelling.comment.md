# CRespelling.cs

## `public sealed class CRespelling`

The respelling gates: which form of a reading a driver prints, and what stands around it.
Every notation rule lives here, so no driver chooses a bracket or a slash.
It stands on the atelier's phonology port, and `CAtelier` builds the one instance every driver shares.

## `internal CRespelling(CAtelier atelier)`

Only the atelier builds it, so each session has one.

## `public bool CRespellingCheck(string language)`

Whether the switch shows respellings for the language.

## `public bool CRespellingPhonemicCheck(string language)`

Whether the language's pack marks its respelling as phonemic.

## `public CRespellingMark CRespellingMarkRead(string language)`

The mark a pronunciation row prints under.
A phonemic respelling stands between slashes, and anything else between square brackets.
The phonemic flag is asked only while respellings are shown.

## `public CRespellingMark CRespellingMarkRead(string language, bool schemed)`

The mark a transcription candidate prints under.
A schemed search prints the phonetic bare, whatever the switch shows.
Otherwise it is the pronunciation mark.

## `public IReadOnlyList<CReflex> CRespellingReflexScan(string language, IReadOnlyList<CReflexDraft> reflexes)`

The reflex rows of an entry in `language`, in order, each ready to show.
One engine read answers every row's switch, phonemic mark and fold.
Each row is marked when it leads its run of one language.
The editor and the reading view both read their rows here.

## `private static CReflex LRespellingReflexRead(CReflexDraft reflex, LReflexGuise guise, bool lead)`

One reflex row under the mark its guise picks.
A phonemic language stands between slashes whatever the switch shows, and any other reading stands bare.
The text is resolved by `CRespellingResolve`, the one rule the accent rows share.

## `public static CAccent CRespellingAccentRead(CRespellingMark mark, string language, CPronunciationDraft spoken)`

One accent row of a draft in the language, ready to show under the mark.
The text is already the form the mark picks, so no driver resolves a respelling for the row.

## `public static string CRespellingResolve(CRespellingMark mark, CPronunciationDraft spoken)`

The form of a draft pronunciation to print under the mark.

## `public static string CRespellingResolve(CRespellingMark mark, string phonetic, string? respelling)`

The respelling while the mark shows it and it is filled, and the phonetic otherwise.
