# Shengfu

This folder holds the phonetic series of Han characters, called Shengfu, and the Stem categories made from them.
A series names the character whose sound a graph was borrowed for, such as 工 for 江.
It stands apart because a series hangs off a character, not off an entry.
Every entry written with the character shares it.

## `LShengfu`

The series one Han character belongs to, as one stored row per language and character.
The text is the series as printed, with several joined by the rule's separator.
The source is printed nowhere and kept for provenance.

## `LShengfuTextFind`

The series of a character among the rows, or empty when none was stored.

## `LShengfuRule`

The pack section that says where a language's phonetic series are read from.
The engine holds no series of its own, so a pack without this section prints none.
The URL is fetched per character, with `{word}` standing for it.
The pattern's `shengfu` group carries one series.
The form fields are posted instead of a plain fetch, and empty means a fetch.
The busy regex marks a refusal page, so nothing is stored.
The interval is the seconds left between two fetches of this source.
The separator joins several series of one character.

## `LShengfuRuleGroup`

The regex group name the pattern carries each series in.

## `LShengfuSource`

The fetch port the engine reads a character's phonetic series through.
It says nothing about HTTP, and `LShengfuSourceHttp` in Infrastructure is its adapter.
It answers the series, or null when the source named none.
The second value tells whether the source answered at all, so a failed fetch is retried later.

## `LStem`

One phonetic series of a language as a category of its own.
The Shengfu rows of the characters that belong to it link to this row.
It is derived from the series text rather than from placements, so it needs no Hypothesis.
The key is the series as the source printed it, such as 工.
The count is the number of entries whose headword carries one of its characters.
The chosen flag is true while the column shows this series as the chosen one.

## `LStemKeyScan`

The series keys a stored series text holds, cut on the separator the language rule prints them with.
An empty separator leaves the text whole.
Blank pieces and repeats are dropped, so one character never joins the same series twice.

## `LStemMember`

One member character of a phonetic series, as the series page shows it.
It carries the character, its representative readings and the reflex rows of that entry.
A member with no entry carries no reflexes, and shows as the bare character with its readings if any.
The readings come from the representative fanqie rows, in rank order.
The reflex rows are in the pack's reflex order, as the entry page reads them.

## `LStemMemberGuises`

The guises of the reflex rows, never null, each carrying its row's shown text.
Application builds the member without them, since the respelling switch is a setting only the engine reads.
The engine adds them from the settings, by position.

## `LStemMemberOpened`

Whether this member's "More readings" fold is opened on this series page.
`LStemClerk` reads it from its own store, apart from the entry page's fold.

## `LStemMemberReading`

The representative line, such as `/ʔak, ʔo, ʔoh/`, or empty when no reading is marked.
It is laid out by `LFanqieGroup.LFanqieReadingFormat`, the owner the entry page's line uses.

## `LStemMemberRows`

Each reflex row paired with its guise, in row order, ready for a reflex table.
The guise at the row's position is read through `LReflexGuiseFind`, so a row without one pairs with null.
The Outpost series sheet writes these rows, where the guises are empty since only the engine reads them.

## `LStemBareScan`

One bare member per character, in the characters' order, with no entry, no readings and no reflexes.
It is the member list of a series no store was read for.

## `LStemMemberFind`

The first entry whose trimmed headword is exactly the character, or null.
The entries come in id order, so of two entries with the same headword the older one answers.

## `LStemPage`

The page of one Stem series as the engine composes it.
It carries the language, the series key and the characters that belong to it.
The characters are already ordered for printing.
The blank page stands for no series, so the veneer never branches on null.

## `LStemPageMembers`

The members of the series, never null.
A page built without members gets one bare member per character through `LStemBareScan`.
So every character has its member, and the blank page carries none.

## `LStemPageEmpty`

True while the series holds no character to print.

## Stamp

Commit: `c2f335c331fe`
- `LShengfu.cs` `29ee968017e0bb07`
- `LShengfuRule.cs` `99b4ac853bd0d108`
- `LShengfuSource.cs` `2481ad03add835ef`
- `LStem.cs` `b5fadbe183372f8b`
- `LStemMember.cs` `0abe15132025970d`
- `LStemPage.cs` `25b63dce5b0b522e`
