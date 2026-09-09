# LRegisterDraft.cs

## `public sealed record LRegisterDraft(`

One Register as a card draft carries it.
It holds the id of the Register the row edits, its name, the language it belongs to, and whether a language pack ships it.
The draft never owns the name, because the id is the reference.
An empty id means the row has not been stored as a Register yet.
The name carries what is known about it.
A row standing empty because nothing was written is never confused with another case.
That case is a row standing empty because what was written cannot be read back.

A Register a language pack ships is never renamed by a card.
The card marks it and reads its name back, so the pack stays the authority on what it is called.

**Parameters**

- `LRegisterDraftName` — The name the row shows, and what is known about it.
- `LRegisterDraftId` — The id of the Register the row edits, empty until one is given.
- `LRegisterDraftLanguage` — The language the Register belongs to, empty when none is stated.
- `LRegisterDraftBuiltin` — True when a language pack ships the Register rather than the user writing it.

## `public static LRegisterDraft LRegisterDraftCreate(string text)`

A name written with nothing else said about it, and no id yet.
It belongs to no language and is not one a language pack ships.
