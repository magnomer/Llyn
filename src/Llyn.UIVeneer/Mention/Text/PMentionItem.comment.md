# PMentionItem.cs

## `internal sealed record PMentionItem`

One row of the menu that opens at a clicked word.
It carries display strings and ids only, so the menu never reads the engine.
In Entry mode a row stands for one candidate Entry: its headword, its language and the flag of that language.
In Sense mode a row stands for one Meaning of one Entry.
It stands for the whole Entry when its sense is zero.
The flag comes from `PEnsign`, so the rows read as every other headword list does.

## `public Thickness PMentionItemIndent`

A sub-sense is listed flat under its parent and pushed right by one step per level.
The template binds this, because a margin is the one thing markup cannot compute from a depth.

## `internal static IReadOnlyList<PMentionItem> PMentionItemCreate(IReadOnlyList<LTranslationTarget> targets)`

The Entry-mode rows, one per candidate, in the order the engine returned them.

## `internal static IReadOnlyList<PMentionItem> PMentionItemCreate(long entryId, IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> senses, string whole)`

The Sense-mode rows, led by one row for the whole Entry.
The senses arrive named and ordered by `LWindow`, so the record only wraps them.
The word for the whole Entry is handed in.
The record has no window to read a key from.

