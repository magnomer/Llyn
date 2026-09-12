# LMeaningArchive.cs

## `public sealed class LMeaningArchive`

Persists the Meaning tree an entry owns.
Each meaning is a stable-id node assigned an opaque id here on creation.
It may nest under another meaning in the *same* entry.
That parent link is validated before the row is written.
Reordering rewrites a meaning's `position` only, so ids never change.
Deleting a meaning removes its subordinate meanings through the foreign-key cascade, and deleting the entry removes the whole tree.

Sibling order is a unique index, so a position is never written one row at a time.
A new meaning is appended to the end of its sibling group.
`LMeaningMove` renumbers the whole group through `LDatabaseOrder`.
Updating a meaning therefore changes its content and nothing about where it sits.

## `public LMeaningArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LMeaning LMeaningCreate(LMeaning meaning)`

Inserts `meaning` with a fresh opaque id at the end of its sibling group.
Returns the stored meaning with that id and its assigned position filled in.
When it names a parent, the parent must be an existing meaning in the same entry.
Otherwise an `InvalidOperationException` is thrown before anything is written.

## `public IReadOnlyList<LMeaning> LMeaningRead(long entryId)`

Reads the entry's whole Meaning tree as a flat list ordered by parent then position.
So a caller rebuilds the hierarchy from each meaning's `LMeaning.LMeaningParentId`.
Siblings arrive already in order.

## `public LMeaning? LMeaningSingleRead(long id)`

Reads the single meaning identified by `id`, or `null` when no meaning carries that id.
That is the existence check a caller needs before pointing a relation at a Meaning.

## `public IReadOnlyList<LMeaning> LMeaningFind(string query)`

Returns the meanings whose owning entry's headword contains `query`.
They are ordered by that headword and then by the meaning's place in its entry.
It returns every meaning when `query` is empty or all whitespace.
This is the meaning-level twin of `LEntryFind`.
A caller holding typed text gets back the Meanings that text could name, and picks one.
No store invents a target for words nobody matched.

Matching is a contains whose case is folded over the whole of Unicode.
It is the same as `LEntryFind`, so an accented headword is found typed in either case.
Sub-meanings are included: a relation may point at any Meaning, not only a top-level one.

## `public void LMeaningUpdate(LMeaning meaning)`

Updates the title, gloss, definition (with its language), and labels of the meaning identified by `meaning`'s id.
The id, entry, parent link, and position are untouched.
Where a Meaning sits among its siblings is changed by `LMeaningMove`.
That method has to renumber the whole group.
Throws when no meaning carries that id.

## `public void LMeaningParentUpdate(long id, string? parentId)`

Moves a Meaning under another Meaning of the same entry, or out to the entry's roots.
The row goes to the end of the group it joins, and the group it left is renumbered behind it.
The unique sibling index rejects two rows sharing a place, so neither group is left with a gap.
A Meaning cannot be moved inside its own subtree, because a Meaning is not its own ancestor.
Nothing happens when the row already sits under that parent.

## `public void LMeaningMove(long id, int position)`

Moves the meaning identified by `id` to `position` among its siblings.
It renumbers the whole group so positions stay `0 … n-1`.
A position outside the group is clamped into it.
Nothing moves when no meaning carries that id.

## `public void LMeaningDelete(long id)`

Deletes the meaning identified by `id` together with everything it owns.
That is its inline definition field, held in columns of the row itself.
It is also the relations originating from it.
It is also its subordinate meanings, with the same treatment applied down the tree.
It is also its example, tag, and situation association rows.
Sibling and ancestor meanings are untouched and are renumbered so positions stay contiguous.
The independent Examples, Tags, and Situations it referenced are left standing.
Only the links go.

Guarded where a cascade must not decide alone.
A relation outside the deleted subtree may still point at one of these meanings.
So may any collocation synonym.
While one does, nothing is deleted and an `InvalidOperationException` is thrown.
Links originating inside the subtree are cleared as part of the delete.
They belong to rows that are going anyway.

## Inline notes

### `private const string LMeaningSubtreeQuery =`

The meanings this delete removes: the named one and every meaning beneath it, walked with a recursive term over parent_id.
Both link statements below start from this set.

### `private static (string? Entry, string? Parent) LMeaningHolderRead(SqliteConnection connection, long id)`

The entry a meaning belongs to and the parent it hangs from.
Those are the two values that name its sibling group.
Both are null when no meaning carries the id.

### `private static void LMeaningCycleValidate(`

A Meaning may not be moved under a Meaning that already sits inside it.
The subtree query answers whether the proposed parent is one of the row's own descendants.
Such a move would cut the branch off the entry and leave a ring the tree walk never ends on.

### `private static IReadOnlyList<string> LMeaningSiblingRead(`

The ids of one sibling group in order.
Root meanings have no parent id to key on.
So their group is the entry's parentless meanings.
That is the same group the ifnull() expression index treats as one.

### `private static void LMeaningLinkValidate(SqliteConnection connection, long id)`

Counts the links reaching the subtree from outside it.
Those are a relation held by a meaning that survives, and any collocation synonym.
A collocation is not deleted when a meaning is.
So every synonym pointing here counts as an outside link.
The schema keeps those columns cascade-free on purpose.
This turns the foreign-key error they would raise into a message that names the reason.

### `private static void LMeaningLinkClear(SqliteConnection connection, long id)`

Clears the target rows of the relations the subtree owns before the meanings go.
They would cascade with their relation anyway.
But a relation inside the subtree pointing at another meaning in the subtree is a problem.
It would be checked against a row already being deleted.
The order the cascade visits tables in is not ours to rely on.
