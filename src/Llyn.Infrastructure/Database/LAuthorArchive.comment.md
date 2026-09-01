# LAuthorArchive.cs

## `public sealed class LAuthorArchive`

Persists Authors — independent data no Reference owns. An Author is created once with an opaque id and is then *referenced* by any number of References through `source_author`, which carries the position the Author takes on that Reference alone. Renaming rewrites the visible name and never the id, so every Reference keeps pointing at the same Author, and `LAuthorDelete` refuses to run while any Reference still credits it.

Attaching an Author to a Reference belongs to `LReferenceArchive`, which owns the association rows; this store only creates, reads, renames, and deletes the Authors themselves.

## `public LAuthorArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LAuthor LAuthorCreate(LAuthor author)`

Inserts `author` with a fresh opaque id and returns the stored Author with that id filled in. The new Author is credited on no Reference until one attaches it.

## `public LAuthor? LAuthorRead(string id)`

Reads the Author identified by `id`, or `null` when none exists.

## `public IReadOnlyList<LAuthor> LAuthorReferenceRead(string referenceId)`

Reads the Authors a Reference credits, in the order that Reference gives them. Another Reference crediting the same Authors may order them differently — the order lives on the association row.

## `public void LAuthorUpdate(LAuthor author)`

Rewrites the name of the Author identified by `author`'s id. The id and every Reference crediting it are untouched, so a rename never changes where the Author appears. Throws when no Author carries that id.

## `public void LAuthorDelete(string id)`

Deletes the Author identified by `id`. Guarded: while any Reference still credits the Author, nothing is deleted and an `InvalidOperationException` is thrown — detach the Author from every Reference first. Deleting an Author never deletes a Reference. The guard and the delete share one transaction, so nothing can start crediting the Author between them.
