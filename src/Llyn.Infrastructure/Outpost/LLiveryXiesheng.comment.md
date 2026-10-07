# LLiveryXiesheng.cs
Hash: `b31e5584d7e63150`

## `internal static class LLiveryXiesheng`

Writes the body of a series note for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryStem`, the `note` map and the `lookup` it is handed.
It also holds the character chip and the entry list the category note shares.

## `public static void LLiveryXieshengAppend(StringBuilder sheet, LLiveryStem stem, Func<long, string> note, Func<string, string> lookup)`

Writes a `llyn-series` chip read through `Xiesheng.Stem`, then the series key as the heading under it.
The member characters follow in page order inside a `llyn-card` div, each through `LLiveryCharacterFormat`.
The space after the last character is dropped only when one was written, so no other text is cut.
A series without characters writes `Xiesheng.StemEmpty` as a `llyn-vacant` span instead.
`LLiveryEntryAppend` writes the entries of the series last, with `Xiesheng.KindredVacant` for none.

## `internal static string LLiveryCharacterFormat(string character, IReadOnlyList<LEntry> entries, Func<long, string> note)`

A `llyn-stem` chip around `character`, linked through `LLiveryEtymology.LLiveryLinkFormat`.
The link targets the entry in `entries` whose headword equals the character, the lowest id first.
A character without such an entry, or with an empty note id, stays plain text.
`LLiveryYunjing` reuses it for the characters of each line.

## `internal static void LLiveryEntryAppend(StringBuilder sheet, IReadOnlyList<LEntry> entries, Func<long, string> note, string vacant)`

Writes one Markdown list line per entry, its headword linked through `LLiveryEtymology.LLiveryLinkFormat`.
The entries keep the order the record holds.
No entries write `vacant` as a `llyn-vacant` span, or nothing when `vacant` is empty.
`LLiveryYunjing` passes an empty `vacant`, since a category note has no text for that case.
