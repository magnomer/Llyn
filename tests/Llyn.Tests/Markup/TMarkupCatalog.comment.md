# TMarkupCatalog.cs

## `public sealed class TMarkupCatalog`

Covers the document a Llyn Markup file reads as: its root, its catalog, and its keys.
The scanner and the card reader under it are covered in `TMarkupSyntax.cs`.
What is pinned here is the shape section 2 of `docs-work/FormatLlynMarkup.md` gives a file.
A `<llyn>` root, a catalog of shared rows under author-chosen keys, and entries that cite them.
Every refusal the format states about the document, the catalog, or a key is pinned here too.
None of these tests touch a workspace, because reading is pure.

## Inline notes

### `private const string TMarkupSample`

One document holding a row of every kind the catalog declares.
It is close to the worked example of section 10, so reading it is close to a promise about that file.

### `MarkupEntryRead_EveryKey_CarriesTheKindDeclared`

The key namespace is one namespace and holds entry and sense keys beside the catalog's rows.
A citation is checked against the kind found here, so this is what makes a wrong kind detectable.

### `MarkupEntryRead_AuthorAfterSource_ResolvesIt`

Section 10 declares `gaskell` after the source that credits him, and says that is allowed.
The catalog is read whole before any citation in it is resolved, and this pins that order.

### `MarkupEntryRead_NestedSenseKey_KeepsBothKeys`

A sub-sense declares a key like any other sense, and both reach the namespace.
Reading the fields of a nested sense is the card reader's work and is not pinned here.

### `MarkupEntryRead_WrongKind_ThrowsNamingBothKinds`

`<situation ref="oed">` where `oed` is a source is the example section 2 gives.
The message names the key, the kind it found, and the kind the citation wanted.
An author with two of those three would have to guess which end is wrong.

### `MarkupEntryRead_NoRootElement_Throws`

A file in the retired format opens with `<entry>` and does not import.
Llyn is before 1.0.0 and keeps no compatibility with formats it has retired.

### `MarkupEntryRead_EntryOutsideRoot_ReturnsNoEntry`

An entry written beside the root rather than inside it is not part of the document.
It is skipped rather than refused, because text outside the root is not markup Llyn reads.

### `Assert.Contains("2", failure.Message);`

A file may hold many entries, so a missing headword must say which entry lacked it.
The number is the entry's place in the file counted from one.
