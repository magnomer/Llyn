# PMentionItem.cs

## `internal sealed record PMentionItem`

One row of the menu that opens at a clicked word.
It carries display strings and ids only, so the menu never reads the engine.
In Entry mode a row stands for one candidate Entry: its headword, its language and the flag of that language.
In Sense mode a row stands for one Meaning of one Entry.
It stands for the whole Entry when its sense is zero.
The flag comes from `QEnsignImage`, so the rows read as every other headword list does.

## `public Thickness PMentionItemIndent`

A sub-sense is listed flat under its parent and pushed right by one step per level.
The template binds this, because a margin is the one thing markup cannot compute from a depth.

## `internal static IReadOnlyList<PMentionItem> PMentionItemCreate(IReadOnlyList<CTranslationTarget> targets)`

The Entry-mode rows, one per candidate, in the order the engine returned them.

## `internal static IReadOnlyList<PMentionItem> PMentionItemCreate(IReadOnlyList<CMeaning> senses)`

The Sense-mode rows, one per row of the ready sense menu.
Conduct names and orders them, the whole-Entry row first, so the record only wraps them.
A Sense-mode pick reads only the sense, so these rows carry no Entry.

