# LMentionDraft.cs

## `public sealed record LMentionDraft(`

One Mention as a draft carries it.
It holds the same five facts as [LMention](LMention.comment.md), under the draft prefix.
The id is negative for a row the database has not stored yet.
It is zero for a row the engine has not minted yet.
The span counts code points, as the stored row does.

**Parameters**

- `LMentionDraftId` — The id of the stored Mention the row edits.
  It is negative for a new row and zero for one not yet minted.
- `LMentionDraftOffset` — The code-point offset the word begins at.
- `LMentionDraftLength` — The number of code points the word occupies.
- `LMentionDraftEntry` — The Entry the word stands for, or zero when it stands for nothing.
- `LMentionDraftSense` — The one Meaning the word is narrowed to, or zero when none is chosen.

## `public static LMentionDraft LMentionDraftCreate(LMention mention)`

The stored Mention as a draft, id kept.
A row picked or loaded from the store enters a draft this way, so a word already linked stays linked.

## `public LMention LMentionDraftResolve()`

The draft as a Mention, id kept, negative for a row the store has not written.

## `public static IReadOnlyList<LMentionDraft> LMentionDraftSort(IReadOnlyList<LMentionDraft> mentions)`

The same drafts ordered by where they start.
