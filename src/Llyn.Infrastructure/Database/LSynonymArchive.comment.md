# LSynonymArchive.cs

## `public sealed class LSynonymArchive`

Persists the synonym interlinks a Collocation owns.
A synonym is a stable-id row assigned an opaque id here on creation.
It hangs from its origin collocation and points at exactly one target.
The target is an Entry or a Meaning, reached through the job05 discriminated target model.
The single-target rule is enforced before anything is written, and again by the table's check constraint.
Deleting the origin collocation removes its synonyms through the foreign-key cascade.
The referenced Entry or Meaning is left intact.

A new synonym is appended to the end of its collocation's order.
`LSynonymMove` renumbers the whole set.
So the caller never writes a position into a set that has to stay contiguous.

TODO: this is the storage seam only.
The precise targeting rules for a collocation synonym are not finalized (see `LSynonym`).
Beyond the XOR, no rule about which targets are legal is enforced yet.

## `public LSynonymArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LSynonym LSynonymCreate(LSynonym synonym)`

Inserts `synonym` with a fresh opaque id at the end of its collocation's order.
Returns it with that id and its assigned position filled in.
Exactly one of the target ids must be set.
Naming both or neither throws an `InvalidOperationException` before anything is written.

## `public IReadOnlyList<LSynonym> LSynonymRead(string collocationId)`

Reads the synonym interlinks hanging from the collocation identified by `collocationId`, ordered by position.
Each comes with its single target resolved to a target Entry id or a target Meaning id.

## `public void LSynonymUpdate(LSynonym synonym)`

Re-points the synonym identified by `synonym`'s id at the target it now names.
A synonym holds nothing but its target, so this is the whole of an update.
The id, the origin Collocation, and the position are untouched.
Where a synonym sits among its siblings is changed by `LSynonymMove`.
That method has to renumber the whole set.
Exactly one of the target ids must be set.
Naming both or neither throws an `InvalidOperationException` before anything is written.
So does an id no synonym carries.

## `public void LSynonymMove(string id, int position)`

Moves the synonym identified by `id` to `position` in its collocation's order.
It renumbers the whole set so positions stay `0 … n-1`.
A position outside the set is clamped into it, and nothing moves when no synonym carries that id.

## `public void LSynonymDelete(string id)`

Deletes the synonym interlink identified by `id`.
The Entry or Meaning it pointed at is untouched.
The synonyms left under the same collocation are renumbered so positions stay contiguous.

## Inline notes

### `private static string? LSynonymHolderRead(SqliteConnection connection, string id)`

The collocation a synonym hangs from, or null when no synonym carries the id.

### `private static IReadOnlyList<string> LSynonymSiblingRead(SqliteConnection connection, string collocationId)`

The synonyms of one collocation, in order.
