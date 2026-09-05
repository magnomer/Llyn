# LSituationDraft.cs

## `public sealed record LSituationDraft(`

One Situation as a card draft carries it.
It holds the id of the Situation the row edits and the wording shown.
The draft never owns the wording, because the id is the reference.
An empty id means the row has not been stored as a Situation yet.
The wording carries what is known about it.
A row standing empty because nothing was written is never confused with another case.
That case is a row standing empty because what was written cannot be read back.

**Parameters**

- `LSituationDraftText` — The wording the row shows, and what is known about it.
- `LSituationDraftId` — The id of the Situation the row edits, empty until one is given.

## `public static LSituationDraft LSituationDraftCreate(string text)`

A wording written with nothing else said about it, and no id yet.
