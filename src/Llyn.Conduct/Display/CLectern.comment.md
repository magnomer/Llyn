# CLectern.cs
Hash: `d643171a2d243beb`

## `public sealed record CLectern(string CLecternHeadword, string CLecternLanguage, IReadOnlyList<string> CLecternSpeeches, bool CLecternMarked, IReadOnlyList<CMarkdownBlock> CLecternNote, bool CLecternNoted, string CLecternAdded, string CLecternUpdated, bool CLecternStamped, string CLecternUnit)`

The header of the reading view for the shown entry, ready to show.
The display builds it once when an entry opens, so the driver only draws it.

**Parameters**

- `CLecternHeadword`: the headword as stored.
- `CLecternLanguage`: the entry's language, which picks the headword font and the flag.
- `CLecternSpeeches`: the names of the parts of speech a reader reads, in order.
- `CLecternMarked`: whether the entry carries a part of speech or a lexical unit, which shows the speech section.
- `CLecternNote`: the note's Markdown blocks, parsed once when the entry opens.
- `CLecternNoted`: whether the note holds text, which shows the note section.
- `CLecternAdded`: the creation time in local short form.
- `CLecternUpdated`: the last update time in local short form.
- `CLecternStamped`: whether the stored entry was read, which shows the stamp row.
- `CLecternUnit`: the localization key of the entry's lexical unit, or empty when none is chosen.
