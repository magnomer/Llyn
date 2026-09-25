# LAuthorVault.cs

## `public interface LAuthorVault`

The persistence port for the Author rows the engine reads and writes.
It lists exactly what the engine asks of author storage, and nothing about how rows are kept.
`LAuthorArchive` in Infrastructure is its adapter over the workspace database.

## `LAuthor LAuthorCreate(LAuthor author);`

Inserts `author` with a fresh opaque id and returns the stored Author with that id filled in.
The new Author is credited on no Reference until one attaches it.

The name may be empty, because section 6 of the format spec lets an author carry none.
An import writes such a row and the engine refuses one typed by hand.

## `LAuthor? LAuthorRead(long id);`

Reads the Author identified by `id`, or `null` when none exists.

## `IReadOnlyList<LAuthor> LAuthorReferenceRead(long referenceId);`

Reads the Authors a Reference credits, in the order that Reference gives them.
Another Reference crediting the same Authors may order them differently — the order lives on the association row.

## `IReadOnlyList<LAuthor> LAuthorAllRead();`

Reads every Author the workspace holds, by name.
The Sources panel offers them for crediting, so it asks for the whole shelf at once.

## `IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LAuthorReferenceRead();`

Reads the credits of every Reference at once, each in that Reference's own order.
A catalog showing credits on every row would otherwise run one query per row.

## `void LAuthorUpdate(LAuthor author);`

Rewrites the name of the Author identified by `author`'s id.
The id and every Reference crediting it are untouched, so a rename never changes where the Author appears.
Throws when no Author carries that id.

## `void LAuthorAbsorb(long kept, long dropped);`

Folds the Author `dropped` into the Author `kept` and deletes the dropped row.
Every Reference crediting the dropped Author credits the kept one afterwards, at the same position.
A Reference already crediting both keeps the kept credit where it stood and loses the duplicate.
The remaining credits of each touched Reference are renumbered, because the order is a unique index.
The kept Author is inserted past every live position first, so the renumbering never collides.
Throws when either id names no Author or both name the same one.
The whole fold is one transaction, so no Reference is left crediting a deleted Author.

## `void LAuthorDelete(long id, bool detach);`

The same delete, with `detach` dropping every credit first.
Dropping a credit renumbers that Reference's remaining credits, because the order is a unique index.
Deleting an Author never deletes a Reference.

## `IReadOnlyList<LUsage> LAuthorUsageRead(long id);`

Reads the Meanings, Collocations and Examples citing any Source the Author is credited on.
A card names its own id, the Entry it belongs to, that Entry's headword, and its title.
An Example names its sentence and its first Gloss, because an Example belongs to no Entry of its own.
The first Gloss is joined by position zero, and an Example without one reads as unspecified.
A card is listed once however many credited Examples it holds, because the row leads to the card.

The count of an Author is the Example rows, which are the citations themselves.
The card rows are the way up to those citations rather than additions to them.
Counting cards and Examples together would count one citation twice, as `LReferenceUsage.comment.md` records.
