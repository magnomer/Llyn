# LMarkupTarget.cs
Hash: `05bdbd40d0434f3b`

## `public sealed record LMarkupTarget(long LMarkupTargetId, int LMarkupTargetMeaning, int LMarkupTargetCollocation)`

A stored entry a parsed entry may join, sharing its headword and language.
It carries what a replacement would drop, so the import window needs no second read.

**Parameters**

- `LMarkupTargetId` — The id of the stored entry.
- `LMarkupTargetMeaning` — The meaning cards the stored entry holds, nested cards counted.
- `LMarkupTargetCollocation` — The collocation cards the stored entry holds, nested cards counted.

## `public static LMarkupTarget LMarkupTargetCreate(long id, LEntryDraft draft)`

The target for a stored entry, its cards counted by `LCardDraftTally`.
