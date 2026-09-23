# LExampleDraft.cs

## `public sealed record LExampleDraft(`

One Example as a draft carries it.
It holds the id of the Example the row edits and the sentence shown.
It also holds what renders the sentence and the Reference it cites.
The draft never owns the sentence, because the id is the reference.
An empty id means the row has not been stored as an Example yet.
Two cards quoting one sentence carry one id, and the save writes one stored row.
The sentence, its rendering and the citation each carry what is known about them.
A row standing empty because nothing was written is never confused with another case.
That case is a row standing empty because the user marked it as not known.

The frame a card reads the sentence under is not here.
A marker and a role belong to the card holding the Example, never to the shared sentence.
`LSentenceDraft` carries them.

**Parameters**

- `LExampleDraftText` — The sentence the row shows, and what is known about it.
- `LExampleDraftId` — The id of the Example the row edits, empty until one is given.
- `LExampleDraftReference` — The Source the sentence cites, and what is known about it.
- `LExampleDraftLanguage` — The language the sentence is written in, empty when none is stated.
- `LExampleDraftGloss` — The sentence rendered in other languages, as [LGlossDraft](LGlossDraft.comment.md) rows in the order the Example keeps them.
- `LExampleDraftMention` — The words of the sentence that stand for an Entry, as [LMentionDraft](LMentionDraft.comment.md) rows ordered by start.

## `public bool Equals(LExampleDraft? other)`

Two drafts are equal when every field is equal and the Gloss and Mention lists match row by row.
A record compares a list by reference, which would make every load a change.

## `public override int GetHashCode()`

The hash that agrees with that equality.

## `public static LExampleDraft LExampleDraftCreate(string text)`

A sentence written with nothing else said about it.
It has no id yet, no rendering, no Source cited, and no language stated.

## `public LExampleDraft LExampleDraftNormalize()`

The same example with every unreadable value dropped to unspecified and its Mentions sorted by start.
Called only after the user agreed to lose what the store could not read.

## `public LMentionDraft? LExampleDraftFind(LMentionDraft span)`

The Mention of the sentence the span lies inside, or none.
It asks `LMentionSpan.LMentionSpanFind`, so a sentence and a narrative match a selection alike.
