# LStemMember.cs
Hash: `0749b81d830fc52b`

## `public sealed record LStemMember(string LStemMemberCharacter, IReadOnlyList<string> LStemMemberReadings, IReadOnlyList<LReflexDraft> LStemMemberReflexes, IReadOnlyList<LReflexGuise>? LStemMemberGuises = null)`

One member character of a phonetic series, as the series page shows it.
It carries the character, its representative readings and the reflex rows of that entry.
A member with no entry carries no reflexes, and shows as the bare character with its readings if any.

**Parameters**

- `LStemMemberCharacter` — The member character.
- `LStemMemberReadings` — The readings of the character's representative fanqie rows, in rank order.
- `LStemMemberReflexes` — The entry's written reflex rows, in the pack's reflex order, as the entry page reads them.
- `LStemMemberGuises` — The guise of each reflex row, by position, which the engine adds from the settings.

## `public IReadOnlyList<LReflexGuise> LStemMemberGuises`

The guises of the reflex rows, never null.
Application builds the member without them, since the respelling switch is a setting only the engine reads.

## `public bool LStemMemberOpened`

Whether this member's "More readings" fold is opened on this series page.
`LStemClerk` reads it from its own store, apart from the entry page's fold.

## `public string LStemMemberReading`

The representative line, such as `/ʔak, ʔo, ʔoh/`, or empty when no reading is marked.
It is laid out by `LFanqieGroup.LFanqieReadingFormat`, the owner the entry page's line uses.

## `public IReadOnlyList<(LReflexDraft, LReflexGuise?)> LStemMemberRows`

Each reflex row paired with its guise, in row order, ready for a reflex table.
The guise at the row's position is read through `LReflexGuise.LReflexGuiseFind`, so a row without one pairs with null.
The Outpost series sheet writes these rows, where the guises are empty since only the engine reads them.

## `public IReadOnlyList<string> LStemMemberLanguages`

The language of each reflex row, in row order, which the engine reads the guises by.

## `public static IReadOnlyList<LStemMember> LStemBareScan(IReadOnlyList<string> characters)`

One bare member per character, in the characters' order: no entry, no readings, no reflexes.
It is the member list of a series no store was read for.

## `public static long? LStemMemberFind(IReadOnlyList<LEntry> entries, string character)`

The first entry among `entries` whose trimmed headword is exactly the character, or null.
The entries come in id order, so of two entries with the same headword the older one answers.
