# LRequest.cs

## `public abstract record LRequest(long LRequestDraftId);`

One edit the UI asks the engine to make to a held Draft.
The UI never assembles a draft.
It sends the draft id, the item, the field and the value, and re-reads what the engine holds.
Each kind of edit is its own record.
So the engine applies it with a type switch and no field flags.
The families live beside this file: the entry-level fields and the card requests.

**Parameters**

- `LRequestDraftId` — The held draft the edit is for.
  The engine refuses a request for a draft it does not hold.
