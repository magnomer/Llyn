# LLiveryXiesheng.cs
Hash: `947573769a393ecb`

## `internal static class LLiveryXiesheng`

Writes the body of a series note for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryStem`, the `note` map and the `lookup` it is handed.
It also holds the character chip and the entry list the category note shares.

## `public static void LLiveryXieshengAppend(StringBuilder sheet, LLiveryStem stem, Func<long, string> note, Func<string, string> lookup)`

Writes a `llyn-series` chip read through `Xiesheng.Stem`, then the series key as the heading under it.
The members of `LStemPageMembers` follow in page order, each in its own card through `LLiveryMemberAppend`.
A series without characters writes `Xiesheng.StemEmpty` as a `llyn-vacant` span instead.
`LLiveryEntryAppend` writes the entries of the series last, leaving out any entry a member card already links.
So every member appears exactly once in the note.
`Xiesheng.KindredVacant` is written only when the series holds no entries at all.

## `private static void LLiveryMemberAppend(StringBuilder sheet, LStemMember member, IReadOnlyList<LEntry> entries, Func<long, string> note)`

Writes one member as a `llyn-card`, as the Series detail pane shows it.
A member with reflex rows opens through `LLiveryCard.LLiveryFoldFormat`, the entry card's fold with its top right chevron.
The fold is open when the ready `LStemMemberOpened` is set, as the Series pane shows it opened.
The summary holds the character through `LLiveryCharacterFormat`, then the ready `LStemMemberReading` in a `llyn-accent` span.
So a folded member keeps its character and reading line in view.
The reading span is left out when the member has no reading.
The reflex rows of `LStemMemberRows` form the folded body through `LLiverySound.LLiveryReadingAppend`, the entry page's reflex table.
No fanqie rows are handed, so the old sound column stays empty.
A member without reflex rows stays a plain `llyn-card` div with no table, since it has nothing to fold.
Every other text is escaped through `LLiveryHeader.LLiveryTextFormat`.

## `internal static string LLiveryCharacterFormat(string character, IReadOnlyList<LEntry> entries, Func<long, string> note)`

A `llyn-stem` chip around `character`, linked as an HTML anchor through `LLiveryEtymology.LLiveryAnchorFormat`.
Joplin parses no Markdown inside a member's `<summary>` line, so a Markdown link would show as literal text.
The link targets the entry in `entries` whose headword equals the character, the lowest id first.
A character without such an entry, or with an empty note id, stays plain text.
`LLiveryYunjing` reuses it for the characters of each line.

## `internal static void LLiveryEntryAppend(StringBuilder sheet, IReadOnlyList<LEntry> entries, Func<long, string> note, string vacant)`

Writes one Markdown list line per entry, its headword linked through `LLiveryEtymology.LLiveryLinkFormat`.
The entries keep the order the record holds.
No entries write `vacant` as a `llyn-vacant` span, or nothing when `vacant` is empty.
`LLiveryYunjing` passes an empty `vacant`, since a category note has no text for that case.
