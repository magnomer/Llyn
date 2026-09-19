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

## `internal static IReadOnlyList<PMentionItem> PMentionItemCreate(long entryId, IReadOnlyList<LMeaning> meanings, string whole, string unknown)`

The Sense-mode rows, led by one row for the whole Entry.
A Meaning is named by its title, or by its definition when it has no title.
The unknown label names it when it has neither.
The words for the whole Entry and for an unknown name are handed in.
The record has no window to read a key from.

## `private static void PMentionItemAppend(...)`

Walks one level of the tree and recurses under each Meaning it lists.
The engine returns the Meanings grouped by parent, and the menu wants them in reading order.
So the children of each parent are picked out and sorted by position before their own children follow.
A row that names itself as its parent is skipped rather than followed.
So a bad row cannot loop the walk.
