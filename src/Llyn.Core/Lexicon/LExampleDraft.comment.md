# LExampleDraft.cs

## `public sealed record LExampleDraft(`

One Example as a draft carries it.
It holds the id of the Example the row edits, the sentence shown, what renders it, and the Reference it cites.
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
- `LExampleDraftTranslation` — The sentence rendered in another language, and what is known about it.
- `LExampleDraftLanguage` — The language the sentence is written in, empty when none is stated.

## `public static LExampleDraft LExampleDraftCreate(string text)`

A sentence written with nothing else said about it.
It has no id yet, no rendering, no Source cited, and no language stated.

## `public LExampleDraft LExampleDraftNormalize()`

The same example with every unreadable value dropped to unspecified.
Called only after the user agreed to lose what the store could not read.
