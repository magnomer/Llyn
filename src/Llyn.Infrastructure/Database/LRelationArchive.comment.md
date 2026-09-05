# LRelationArchive.cs

## `public sealed class LRelationArchive`

Persists the lexical relations a Meaning owns.
Each relation is a stable-id row assigned an opaque id here on creation.
It hangs from its origin meaning and points at exactly one target.
The target is an Entry or another Meaning.
It is reached through a checked reference row in `relation_entry` XOR `relation_sense`.
The single-target rule is enforced before anything is written.
Deleting the origin Meaning removes its relations and their target rows through the cascade.
The referenced Entry or Meaning is left intact.

Order within the origin Meaning is a unique index.
So a position is never written one row at a time.
A new relation is appended to the end.
`LRelationMove` renumbers the whole set through `LDatabaseOrder`.

## `public LRelationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRelation LRelationCreate(LRelation relation)`

Inserts `relation` with a fresh opaque id and its single target row.
It goes at the end of its origin Meaning's order.
Returns the stored relation with that id and its assigned position filled in.
Exactly one of the target ids must be set.
Naming both or neither throws an `InvalidOperationException` before anything is written.

## `public IReadOnlyList<LRelation> LRelationRead(string meaningId)`

Reads the relations originating from the Meaning identified by `meaningId`, ordered by position.
Each comes with its single target resolved to a target Entry id or a target Meaning id.

## `public void LRelationUpdate(LRelation relation)`

Updates the type, label, and labels of the relation identified by `relation`'s id.
The id, origin Meaning, target, and position are untouched.
Where a relation sits among its siblings is changed by `LRelationMove`.
That method has to renumber the whole set.
Throws when no relation carries that id.

## `public void LRelationMove(string id, int position)`

Moves the relation identified by `id` to `position` among the relations of its origin Meaning.
It renumbers the whole set so positions stay `0 … n-1`.
A position outside the set is clamped into it, and nothing moves when no relation carries that id.

## `public void LRelationDelete(string id)`

Deletes the relation identified by `id`.
Its target row is removed by the foreign-key cascade.
The referenced Entry or Meaning is untouched.
The relations left under the same origin Meaning are renumbered so their positions stay contiguous.

## Inline notes

### `private static string? LRelationHolderRead(SqliteConnection connection, string id)`

The Meaning a relation hangs from, or null when no relation carries the id.

### `private static IReadOnlyList<string> LRelationSiblingRead(SqliteConnection connection, string meaningId)`

The relations of one origin Meaning, in order.
