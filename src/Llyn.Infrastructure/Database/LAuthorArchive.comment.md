# LAuthorArchive.cs

## `public sealed class LAuthorArchive`

Persists Authors — independent data no Reference owns.
An Author is created once with an opaque id.
It is then *referenced* by any number of References through `reference_author`.
That row carries the position the Author takes on that Reference alone.
Renaming rewrites the visible name and never the id.
So every Reference keeps pointing at the same Author.
`LAuthorDelete` refuses to run while any Reference still credits it.

Attaching an Author to a Reference belongs to `LReferenceArchive`, which owns the association rows.
This store only creates, reads, renames, and deletes the Authors themselves.

## `public LAuthorArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LAuthor LAuthorCreate(LAuthor author)`

Inserts `author` with a fresh opaque id and returns the stored Author with that id filled in.
The new Author is credited on no Reference until one attaches it.

The name may be empty, because section 6 of the format spec lets an author carry none.
An import writes such a row and the engine refuses one typed by hand.

## `public LAuthor? LAuthorRead(long id)`

Reads the Author identified by `id`, or `null` when none exists.

## `public IReadOnlyList<LAuthor> LAuthorReferenceRead(long referenceId)`

Reads the Authors a Reference credits, in the order that Reference gives them.
Another Reference crediting the same Authors may order them differently — the order lives on the association row.

## `public IReadOnlyList<LAuthor> LAuthorAllRead()`

Reads every Author the workspace holds, by name.
The Sources panel offers them for crediting, so it asks for the whole shelf at once.

## `public IReadOnlyDictionary<string, IReadOnlyList<LAuthor>> LAuthorReferenceRead()`

Reads the credits of every Reference at once, each in that Reference's own order.
A catalog showing credits on every row would otherwise run one query per row.

## `public void LAuthorUpdate(LAuthor author)`

Rewrites the name of the Author identified by `author`'s id.
The id and every Reference crediting it are untouched, so a rename never changes where the Author appears.
Throws when no Author carries that id.

## `public void LAuthorAbsorb(long kept, long dropped)`

Folds the Author `dropped` into the Author `kept` and deletes the dropped row.
Every Reference crediting the dropped Author credits the kept one afterwards, at the same position.
A Reference already crediting both keeps the kept credit where it stood and loses the duplicate.
The remaining credits of each touched Reference are renumbered, because the order is a unique index.
The kept Author is inserted past every live position first, so the renumbering never collides.
Throws when either id names no Author or both name the same one.
The whole fold is one transaction, so no Reference is left crediting a deleted Author.

## `public void LAuthorDelete(long id, bool detach)`

The same delete, with `detach` dropping every credit first.
Dropping a credit renumbers that Reference's remaining credits, because the order is a unique index.
Deleting an Author never deletes a Reference.

## `public void LAuthorDelete(long id)`

Deletes the Author identified by `id`.
Guarded: while any Reference still credits the Author, nothing is deleted.
An `InvalidOperationException` is thrown instead.
Detach the Author from every Reference first.
Deleting an Author never deletes a Reference.
The guard and the delete share one transaction, so nothing can start crediting the Author between them.
