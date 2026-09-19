# LTagVault.cs

## `public interface LTagVault`

The persistence port for the Tag rows the engine reads and writes.
It lists exactly what the engine asks of tag storage, and nothing about how rows are kept.
`LTagArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LTag> LTagMeaningRead(long meaningId);`

Reads the Tags a Meaning carries, in the order that Meaning gives them.

## `IReadOnlyList<LTag> LTagCollocationRead(long collocationId);`

Reads the Tags a Collocation carries, in the order that Collocation gives them.

## `IReadOnlyList<long> LTagMeaningSave(long meaningId, IReadOnlyList<LTag> tags);`

Writes a Meaning's whole Tag line, replacing whatever it carried.
Each Tag is resolved to a row by id, or by trimmed text when it carries no stored id.
Blanks and repeats are dropped, and what survives is numbered from zero in the order given.
The answer holds one row id per Tag handed in, zero for a blank.
The caller maps a draft id to its row from it.

## `IReadOnlyList<long> LTagCollocationSave(long collocationId, IReadOnlyList<LTag> tags);`

The same write for a Collocation.

## `LTag? LTagRead(long id);`

Reads one Tag by id, `null` when no row carries it.

## `long LTagResolve(string text);`

The id of the row reading `text`, created when no row reads it yet.

## `IReadOnlyList<LTag> LTagCatalogRead();`

Reads every Tag row, in alphabetical order.
That is the Tag list itself.

## `void LTagChange(long id, string renamed);`

Renames a Tag everywhere it is linked by rewriting its one row.
When another row already reads `renamed`, the two fold into that row.
A card already carrying both keeps one link rather than gaining a duplicate.
That card's order closes over the link that folded away.

## `void LTagDelete(long id);`

Unlinks a Tag from every card that carries it and closes the gap it leaves in each card's order.
Then it deletes the row.
Nothing else is deleted: a card that carried only this Tag stays, now carrying none.
