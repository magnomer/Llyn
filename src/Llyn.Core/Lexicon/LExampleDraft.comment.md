# LExampleDraft.cs

## `public sealed record LExampleDraft(`

One Example as a card draft carries it.
It holds the id of the Example the row edits, the sentence shown, and the Reference it cites.
The draft never owns the sentence, because the id is the reference.
An empty id means the row has not been stored as an Example yet.
The sentence and the citation each carry what is known about them.
A row standing empty because nothing was written is never confused with another case.
That case is a row standing empty because what was written cannot be read back.

**Parameters**

- `LExampleDraftText` — The sentence the row shows, and what is known about it.
- `LExampleDraftId` — The id of the Example the row edits, empty until one is given.
- `LExampleDraftReference` — The Source the sentence cites, and what is known about it.

## `public static LExampleDraft LExampleDraftCreate(string text)`

A sentence written with nothing else said about it: no id yet, and no Source cited.
