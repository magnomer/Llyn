# LSituationDraft.cs

## `public sealed record LSituationDraft(`

One Situation as a card draft carries it.
It holds the id of the Situation the row edits and the three fields the store keeps.
The draft never owns them, because the id is the reference.
An empty id means the row has not been stored as a Situation yet.
Each field carries what is known about it.
A row standing empty because nothing was written is never confused with another case.
That case is a row standing empty because what was written cannot be read back.

The card shows the title alone, and the other two ride along untouched.
They are stored data, so a save that dropped them would erase what another panel wrote.

**Parameters**

- `LSituationDraftTitle` — The wording the row shows, and what is known about it.
- `LSituationDraftId` — The id of the Situation the row edits, empty until one is given.
- `LSituationDraftDescription` — What the Situation describes at length, and what is known about it.
- `LSituationDraftKind` — What kind of Situation it is, and what is known about it.

## `public static LSituationDraft LSituationDraftCreate(string text)`

A wording written with nothing else said about it, and no id yet.
