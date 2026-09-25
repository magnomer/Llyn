# LSituationDraft.cs

## `public sealed record LSituationDraft(`

One Situation as a card draft carries it.
It holds the id of the Situation the row edits and the three fields the store keeps.
The draft never owns them, because the id is the reference.
An empty id means the row has not been stored as a Situation yet.
Each field carries what is known about it.
A row standing empty because nothing was written is never confused with another case.
That case is a row standing empty because the user marked it as not known.

The card shows the title alone, and the other two ride along untouched.
They are stored data, so a save that dropped them would erase what another panel wrote.

**Parameters**

- `LSituationDraftTitle` — The wording the row shows, and what is known about it.
- `LSituationDraftId` — The id of the Situation the row edits, empty until one is given.
- `LSituationDraftDescription` — What the Situation describes at length, and what is known about it.
- `LSituationDraftKind` — What kind of Situation it is, and what is known about it.

## `public LSituationDraft LSituationDraftNormalize()`

The same situation with every unreadable value dropped to unspecified.
Called only after the user agreed to lose what the store could not read.
