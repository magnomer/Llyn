# LRegisterDraft.cs

## `public sealed record LRegisterDraft(`

One Register as a card draft carries it.
It holds the id of the Register the row marks and the wording shown.
The draft never owns the wording, because the id is the reference.
An empty id means the row has not been stored as a Register yet.
The wording carries what is known about it.
A row standing empty because nothing was written is never confused with another case.
That case is a row standing empty because what was written cannot be read back.

**Parameters**

- `LRegisterDraftText` — The wording the row shows, and what is known about it.
- `LRegisterDraftId` — The id of the Register the row marks, empty until one is given.

## `public static LRegisterDraft LRegisterDraftCreate(string text)`

A wording written with nothing else said about it, and no id yet.
