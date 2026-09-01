# LRelationArchive.cs

## `public sealed class LRelationArchive`

Persists the lexical relations a Meaning owns. Each relation is a stable-id row assigned an opaque id here on creation, hanging from its origin sense and pointing at exactly one target — an Entry or another Meaning — through a checked reference row in `relation_entry` XOR `relation_sense`. The single-target rule is enforced before anything is written. Deleting the origin Meaning removes its relations and their target rows through the foreign-key cascade; the referenced Entry/Meaning is left intact.

Order within the origin Meaning is a unique index, so a position is never written one row at a time: a new relation is appended to the end, and `LRelationMove` renumbers the whole set through `LDatabaseOrder`.

## `public LRelationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRelation LRelationCreate(LRelation relation)`

Inserts `relation` with a fresh opaque id and its single target row at the end of its origin Meaning's order, returning the stored relation with that id and its assigned position filled in. Exactly one of the target ids must be set; naming both or neither throws an `InvalidOperationException` before anything is written.

## `public IReadOnlyList<LRelation> LRelationRead(string senseId)`

Reads the relations originating from the Meaning identified by `senseId`, ordered by position, each with its single target resolved to either a target Entry id or a target Meaning id.

## `public void LRelationUpdate(LRelation relation)`

Updates the type, label, and labels of the relation identified by `relation`'s id. The id, origin Meaning, target, and position are untouched — where a relation sits among its siblings is changed by `LRelationMove`, which has to renumber the whole set. Throws when no relation carries that id.

## `public void LRelationMove(string id, int position)`

Moves the relation identified by `id` to `position` among the relations of its origin Meaning, renumbering the whole set so positions stay `0 … n-1`. A position outside the set is clamped into it, and nothing moves when no relation carries that id.

## `public void LRelationDelete(string id)`

Deletes the relation identified by `id`. Its target row is removed by the foreign-key cascade; the referenced Entry/Meaning is untouched. The relations left under the same origin Meaning are renumbered so their positions stay contiguous.

## Inline notes

### `private static string? LRelationHolderRead(SqliteConnection connection, string id)`

The Meaning a relation hangs from, or null when no relation carries the id.

### `private static IReadOnlyList<string> LRelationSiblingRead(SqliteConnection connection, string senseId)`

The relations of one origin Meaning, in order.
