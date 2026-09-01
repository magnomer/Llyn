# LSynonymArchive.cs

## `public sealed class LSynonymArchive`

Persists the synonym interlinks a Collocation owns. A synonym is a stable-id row assigned an opaque id here on creation, hanging from its origin collocation and pointing at exactly one target — an Entry or a Meaning — through the job05 discriminated target model. The single-target rule is enforced before anything is written, and again by the table's check constraint. Deleting the origin collocation removes its synonyms through the foreign-key cascade; the referenced Entry/Meaning is left intact.

A new synonym is appended to the end of its collocation's order and `LSynonymMove` renumbers the whole set, so the caller never writes a position into a set that has to stay contiguous.

TODO: this is the storage seam only — the precise targeting rules for a collocation synonym are not finalized (see `LSynonym`). Beyond the XOR, no rule about which targets are legal is enforced yet.

## `public LSynonymArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LSynonym LSynonymCreate(LSynonym synonym)`

Inserts `synonym` with a fresh opaque id at the end of its collocation's order and returns it with that id and its assigned position filled in. Exactly one of the target ids must be set; naming both or neither throws an `InvalidOperationException` before anything is written.

## `public IReadOnlyList<LSynonym> LSynonymRead(string collocationId)`

Reads the synonym interlinks hanging from the collocation identified by `collocationId`, ordered by position, each with its single target resolved to either a target Entry id or a target Meaning id.

## `public void LSynonymUpdate(LSynonym synonym)`

Re-points the synonym identified by `synonym`'s id at the target it now names. A synonym holds nothing but its target, so this is the whole of an update: the id, the origin Collocation, and the position are untouched — where a synonym sits among its siblings is changed by `LSynonymMove`, which has to renumber the whole set. Exactly one of the target ids must be set; naming both or neither throws an `InvalidOperationException` before anything is written, and so does an id no synonym carries.

## `public void LSynonymMove(string id, int position)`

Moves the synonym identified by `id` to `position` in its collocation's order, renumbering the whole set so positions stay `0 … n-1`. A position outside the set is clamped into it, and nothing moves when no synonym carries that id.

## `public void LSynonymDelete(string id)`

Deletes the synonym interlink identified by `id`. The Entry/Meaning it pointed at is untouched, and the synonyms left under the same collocation are renumbered so their positions stay contiguous.

## Inline notes

### `private static string? LSynonymHolderRead(SqliteConnection connection, string id)`

The collocation a synonym hangs from, or null when no synonym carries the id.

### `private static IReadOnlyList<string> LSynonymSiblingRead(SqliteConnection connection, string collocationId)`

The synonyms of one collocation, in order.
