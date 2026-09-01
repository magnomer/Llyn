# LReferenceArchive.cs

## `public sealed class LReferenceArchive`

Persists bibliographic References — independent data no Entry, Example, or Author owns. A Reference is created once with an opaque id and is then cited two ways: an Entry cites any number of them in its own order through `entry_source`, and an Example cites at most one through its `example.source_id` column. Both are pointers — detaching a citation leaves the Reference standing, and `LReferenceDelete` refuses to run while any citation remains.

Every field is stored as a state column beside its value column so "never filled in", "recorded as unknown", and "this value" stay three different facts; the value column is written only when the state is specified. The Authors a Reference credits are attached in order through `source_author`: attaching and detaching write association rows only, deleting the Reference drops those rows and never an Author, and the Authors themselves live in `LAuthorArchive`.

Both citation orders are unique indexes, so attaching and detaching renumber the whole set they touch through `LDatabaseOrder`; a caller names the index it wants and never has to find a free position or leave a gap behind.

## `public LReferenceArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LReference LReferenceCreate(LReference reference)`

Inserts `reference` with a fresh opaque id and returns the stored Reference with that id filled in. Each field is written as its state plus, for a specified field alone, its value. The new Reference is cited by nothing and credits no Author until one is attached.

## `public LReference? LReferenceRead(string id)`

Reads the Reference identified by `id`, or `null` when none exists.

## `public IReadOnlyList<LReference> LReferenceEntryRead(string entryId)`

Reads the References an Entry cites, in the order that Entry gives them. Another Entry citing the same References may order them differently — the order lives on the association row.

## `public LReference? LReferenceExampleRead(string exampleId)`

Reads the single Reference an Example cites, or `null` when the Example cites none or does not exist.

## `public void LReferenceUpdate(LReference reference)`

Rewrites every field of the Reference identified by `reference`'s id, states and values alike. The id, the Authors credited, and every citation pointing at it are untouched, so an update never changes where the Reference appears or in what order. Throws when no Reference carries that id.

## `public void LReferenceDelete(string id)`

Deletes the Reference identified by `id` together with the author links it owns. Guarded: while any Entry or Example still cites the Reference, nothing is deleted and an `InvalidOperationException` is thrown — remove those citations first. The Authors it credited survive; only the links between them and this Reference go. The guard and the delete share one transaction, so nothing can cite the Reference between them.

## `public void LReferenceAuthorAttach(string referenceId, string authorId, int position)`

Credits an existing Author on this Reference at `position` in the Reference's own author order. The Author row itself is untouched and stays available to every other Reference.

## `public void LReferenceAuthorDetach(string referenceId, string authorId)`

Removes this Reference's credit for an Author. The Author and its other credits survive.

## `public void LReferenceEntryAttach(string entryId, string referenceId, int position)`

Cites this Reference from an Entry at `position` in that Entry's citation order. An Entry may cite any number of References; each keeps its own position there.

## `public void LReferenceEntryDetach(string entryId, string referenceId)`

Removes an Entry's citation of a Reference. The Reference and its other citations survive.

## `public void LReferenceExampleAttach(string exampleId, string referenceId)`

Makes this Reference the single one an Example cites, replacing whatever it cited before — an Example holds one source column, so there is nothing to order and nothing to duplicate. The Reference is only pointed at, never owned.

## `public void LReferenceExampleDetach(string exampleId)`

Clears the Reference an Example cites. Only the pointer is cleared: both the Example and the Reference stay exactly as they were.

## Inline notes

### `using (SqliteCommand command = connection.CreateCommand())`

source_author cascades from source, so the author links go with the row and the Authors stay.

### `private void LReferenceLinkAttach(`

The two citation tables differ only in their name and their two columns, so both attach and both detach share one implementation. Every identifier here is a store-owned literal chosen by the methods above, never caller input; every value still travels as a parameter.

The row goes in beyond the end of the set and the set is then renumbered around it, so the requested index is honoured and an occupied position is no longer a unique-index failure.
