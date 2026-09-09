# TMarkupImport.cs

## `public sealed class TMarkupImport`

Covers the seam between the markup reader and the store.
A document goes in as text and its catalog comes out as rows that point at each other.
A file with anything broken in it adds nothing at all.

The parse itself is covered in `TMarkupSyntax.cs` and `TMarkupCatalog.cs`.
What is tested here is only what needs a workspace.
That is the rows, their identity, and the atomicity.

## Inline notes

### `private const string TMarkupSample`

One document holding a catalog of every kind and two entries that cite it.
It leaves out the tags the save deliberately drops, so no assertion tests the sample instead of the import.

### `MarkupImport_Catalog_StoresRowsPointingAtEachOther`

The chain the catalog exists for: an author, a source crediting the author, an example citing the source.
Each is stored once, and each stored row names the row the file said it names.
The credits keep the order the source wrote them, because first author is a distinction Llyn keeps.

### `MarkupImport_KeyOfARow_LeavesNoTraceInTheStore`

A key is meaningful only inside its own file and never becomes an identifier.
Two files reusing `oed` for different works would otherwise collide in one workspace.

### `MarkupImport_UncitedSource_StoresItAnyway`

Section 3 keeps a source nothing quotes.
Declaring it is the author's statement that the workspace holds the work.

### `MarkupImport_UncreditedAuthor_StoresItAnyway`

An author credited on nothing had nowhere to live in the old format.
The row is written and no credit row is written with it.

### `MarkupImport_TwoAuthorsOneName_StoresTwoRows`

Author identity comes from the key, not from the name.
The old format deduplicated by name, so two namesakes imported as one person.

### `private static void TMarkupRefusalCheck(string text)`

Every refusal is the same promise: the file is refused and the workspace is untouched.
The catalog is written before the entries, so a refusal must leave no row behind either.
