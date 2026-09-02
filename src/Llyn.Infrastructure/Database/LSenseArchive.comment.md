# LSenseArchive.cs

## `public sealed class LSenseArchive`

Persists the Meaning tree an entry owns.
Each sense is a stable-id node assigned an opaque id here on creation.
It may nest under another sense in the *same* entry.
That parent link is validated before the row is written.
Reordering rewrites a sense's `position` only, so ids never change.
Deleting a sense removes its subordinate senses through the foreign-key cascade, and deleting the entry removes the whole tree.

Sibling order is a unique index, so a position is never written one row at a time.
A new sense is appended to the end of its sibling group.
`LSenseMove` renumbers the whole group through `LDatabaseOrder`.
Updating a sense therefore changes its content and nothing about where it sits.

## `public LSenseArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LSense LSenseCreate(LSense sense)`

Inserts `sense` with a fresh opaque id at the end of its sibling group.
Returns the stored sense with that id and its assigned position filled in.
When it names a parent, the parent must be an existing sense in the same entry.
Otherwise an `InvalidOperationException` is thrown before anything is written.

## `public IReadOnlyList<LSense> LSenseRead(string entryId)`

Reads the entry's whole Meaning tree as a flat list ordered by parent then position.
So a caller rebuilds the hierarchy from each sense's `LSense.LSenseParentId`.
Siblings arrive already in order.

## `public LSense? LSenseSingleRead(string id)`

Reads the single sense identified by `id`, or `null` when no sense carries that id.
That is the existence check a caller needs before pointing a relation at a Meaning.

## `public IReadOnlyList<LSense> LSenseFind(string query)`

Returns the senses whose owning entry's headword contains `query`.
They are ordered by that headword and then by the sense's place in its entry.
It returns every sense when `query` is empty or all whitespace.
This is the meaning-level twin of `LEntryFind`.
A caller holding typed text gets back the Meanings that text could name, and picks one.
No store invents a target for words nobody matched.

Matching is a contains whose case is folded over the whole of Unicode.
It is the same as `LEntryFind`, so an accented headword is found typed in either case.
Sub-meanings are included: a relation may point at any Meaning, not only a top-level one.

## `public void LSenseUpdate(LSense sense)`

Updates the title, gloss, definition (with its language), and labels of the sense identified by `sense`'s id.
The id, entry, parent link, and position are untouched.
Where a Meaning sits among its siblings is changed by `LSenseMove`.
That method has to renumber the whole group.
Throws when no sense carries that id.

## `public void LSenseMove(string id, int position)`

Moves the sense identified by `id` to `position` among its siblings.
It renumbers the whole group so positions stay `0 … n-1`.
A position outside the group is clamped into it.
Nothing moves when no sense carries that id.

## `public void LSenseDelete(string id)`

Deletes the sense identified by `id` together with everything it owns.
That is its inline definition field, held in columns of the row itself.
It is also the relations originating from it.
It is also its subordinate senses, with the same treatment applied down the tree.
It is also its example, tag, and situation association rows.
Sibling and ancestor senses are untouched and are renumbered so positions stay contiguous.
The independent Examples, Tags, and Situations it referenced are left standing.
Only the links go.

Guarded where a cascade must not decide alone.
A relation outside the deleted subtree may still point at one of these senses.
So may any collocation synonym.
While one does, nothing is deleted and an `InvalidOperationException` is thrown.
Links originating inside the subtree are cleared as part of the delete.
They belong to rows that are going anyway.

## Inline notes

### `private const string LSenseSubtreeQuery =`

The senses this delete removes: the named one and every sense beneath it, walked with a recursive term over parent_id.
Both link statements below start from this set.

### `private static (string? Entry, string? Parent) LSenseHolderRead(SqliteConnection connection, string id)`

The entry a sense belongs to and the parent it hangs from.
Those are the two values that name its sibling group.
Both are null when no sense carries the id.

### `private static IReadOnlyList<string> LSenseSiblingRead(`

The ids of one sibling group in order.
Root senses have no parent id to key on.
So their group is the entry's parentless senses.
That is the same group the ifnull() expression index treats as one.

### `private static void LSenseLinkValidate(SqliteConnection connection, string id)`

Counts the links reaching the subtree from outside it.
Those are a relation held by a sense that survives, and any collocation synonym.
A collocation is not deleted when a sense is.
So every synonym pointing here counts as an outside link.
The schema keeps those columns cascade-free on purpose.
This turns the foreign-key error they would raise into a message that names the reason.

### `private static void LSenseLinkClear(SqliteConnection connection, string id)`

Clears the target rows of the relations the subtree owns before the senses go.
They would cascade with their relation anyway.
But a relation inside the subtree pointing at another sense in the subtree is a problem.
It would be checked against a row already being deleted.
The order the cascade visits tables in is not ours to rely on.
