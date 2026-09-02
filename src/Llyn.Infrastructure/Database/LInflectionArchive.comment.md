# LInflectionArchive.cs

## `public sealed class LInflectionArchive`

Persists an entry's inflected forms and the ordered grammatical features each carries.
Inflections are written as ordered child rows under an entry.
So reordering rewrites `position` only and never touches the entry id.
A feature's identity is `(entry_id, inflection_position, position)`.
Deleting an inflection removes its features through the foreign-key cascade, and deleting the entry removes both.
Only stable ids are stored here — feature and value display names are resolved from the morphology vocabulary (`LMorphologyArchive`).

Position is both the order and the key here.
A feature names the inflection it belongs to by that number.
So no single row is ever renumbered on its own.
Removing or moving an inflection rewrites the entry's whole set instead.
That keeps positions at `0 … n-1` and carries every feature along with its inflection.

## `public LInflectionArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public void LInflectionAppend(string entryId, IReadOnlyList<LInflection> inflections)`

Adds `inflections` after the inflections the entry identified by `entryId` already has, each with its features in list order.
The whole write is one transaction.
The new positions continue the entry's existing numbering.
So adding to an entry that already has inflections extends the set rather than colliding.

## `public IReadOnlyList<LInflection> LInflectionRead(string entryId)`

Reads the entry's inflections, ordered by position, each carrying its ordered features.

## `public void LInflectionSet(string entryId, IReadOnlyList<LInflection> inflections)`

Replaces the entry's inflections with `inflections` in list order.
Existing inflection rows are cleared, their features cascade, and the new set is written.
So reordering rewrites positions while the entry id stays fixed.

## `public void LInflectionDelete(string entryId, int position)`

Deletes the single inflection at `position` under `entryId` and closes the gap it leaves.
The inflections that remain keep their order and are renumbered `0 … n-1`.
Their features move with them.
Nothing happens when the entry has no inflection at that position.

## `public void LInflectionMove(string entryId, int position, int target)`

Moves the inflection at `position` under `entryId` to `target`.
It rewrites the whole set so positions stay `0 … n-1`.
Every feature follows its inflection.
A target outside the set is clamped into it.
Nothing moves when the entry has no inflection at that position.

## Inline notes

### `private static IReadOnlyList<LInflection> LInflectionSetRead(SqliteConnection connection, string entryId)`

The entry's inflections in order on a connection the caller already holds.
One query reads the inflections and one reads every feature of all of them.
There is no round-trip per inflection.

### `private static int LInflectionCountRead(SqliteConnection connection, string entryId)`

How many inflections the entry already has, so an append continues its numbering.

### `private static IReadOnlyDictionary<int, IReadOnlyList<LFeature>> LInflectionFeatureRead(`

Every feature of every inflection the entry has, in one query, grouped by the inflection position it belongs to.
