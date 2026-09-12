# LReferenceArchive.cs

## `public sealed class LReferenceArchive`

Persists bibliographic References — independent data no Entry, Example, or Author owns.
A Reference is created once with an opaque id and is then cited one way.
An Example cites at most one through its `example.source_id` column.
An Entry reaches a Reference only through the Examples its cards quote, and holds no citation of its own.
The citation is a pointer — clearing it leaves the Reference standing, and `LReferenceDelete` refuses to run while any citation remains.

Every text field is stored as a state column beside its value column.
So "never filled in", "recorded as unknown", and "this value" stay three different facts.
The value column is written only when the state is specified.
The kind is written as a single word, because its own members already carry those three states.
The Authors a Reference credits are attached in order through `source_author`.
Attaching and detaching write association rows only.
Deleting the Reference drops those rows and never an Author.
The Authors themselves live in `LAuthorArchive`.

The author order is a unique index.
So crediting and dropping a credit renumber the whole set they touch through `LDatabaseOrder`.
A caller names the index it wants.
It never has to find a free position or leave a gap behind.

## `public LReferenceArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LReference LReferenceCreate(LReference reference)`

Inserts `reference` and returns the stored Reference with its id filled in.
A Reference carrying no id is given a fresh opaque one.
A Reference already carrying one keeps it, because a held source is named before the record it becomes exists.
Each field is written as its state plus, for a specified field alone, its value.
The new Reference is cited by nothing and credits no Author until one is attached.

## `public LReference? LReferenceRead(long id)`

Reads the Reference identified by `id`, or `null` when none exists.

## `public LReference? LReferenceExampleRead(long exampleId)`

Reads the single Reference an Example cites, or `null` when the Example cites none or does not exist.

## `public void LReferenceUpdate(LReference reference)`

Rewrites every field of the Reference identified by `reference`'s id, states and values alike.
The id, the Authors credited, and every citation pointing at it are untouched.
So an update never changes where the Reference appears or in what order.
Throws when no Reference carries that id.

## `public void LReferenceDelete(long id)`

Deletes the Reference identified by `id` together with the author links it owns.
Guarded: while any Example still cites the Reference, nothing is deleted.
An `InvalidOperationException` is thrown instead.
Remove those citations first.
The Authors it credited survive.

## `public void LReferenceDelete(long id, bool detach)`

The same delete, with `detach` clearing every citation first.
The detaching, the count, and the delete stand in one session.
Between any two steps the answer to whether anything still cites it can change.
An Example is not deleted with the Reference, so its citation is cleared back to Unspecified.
Only the links between them and this Reference go.
The guard and the delete share one transaction, so nothing can cite the Reference between them.

## `public void LReferenceAuthorAttach(long referenceId, long authorId, int position)`

Credits an existing Author on this Reference at `position` in the Reference's own author order.
The Author row itself is untouched and stays available to every other Reference.

## `public void LReferenceAuthorDetach(long referenceId, long authorId)`

Removes this Reference's credit for an Author.
The Author and its other credits survive.

## `public void LReferenceExampleAttach(long exampleId, long referenceId)`

Makes this Reference the single one an Example cites, replacing whatever it cited before.
An Example holds one source column, so there is nothing to order and nothing to duplicate.
The Reference is only pointed at, never owned.

## `public void LReferenceExampleDetach(long exampleId)`

Clears the Reference an Example cites.
Only the pointer is cleared: both the Example and the Reference stay exactly as they were.

## Inline notes

### `using (SqliteCommand command = connection.CreateCommand())`

source_author cascades from source, so the author links go with the row and the Authors stay.

### `private void LReferenceLinkAttach(`

The two citation tables differ only in their name and their two columns.
So both attach and both detach share one implementation.
Every identifier here is a store-owned literal chosen by the methods above, never caller input.
Every value still travels as a parameter.

The row goes in beyond the end of the set and the set is then renumbered around it.
So the requested index is honoured.
An occupied position is no longer a unique-index failure.
