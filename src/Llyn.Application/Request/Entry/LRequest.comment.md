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

## `public abstract string LRequestKey { get; }`

The field this request writes, so a tenure waiting to flush keeps only the latest request per field.
Every record spells its own key with `nameof`, so no request reads its type at run time.
The plain fields key by type name alone, which suits the entry-level fields, of which there is one each.
A request on one row of a list adds the row's id, so two rows never replace each other.
The structural requests keep the type key and are applied at once rather than deferred.
Two deferred additions would collapse into one, which is why nothing defers them.
