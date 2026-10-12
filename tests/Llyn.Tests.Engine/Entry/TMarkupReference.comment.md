# TMarkupReference.cs
Hash: `858c8526b703ab1f`

## `public sealed class TMarkupReference`

Covers the references a markup import creates or reuses for its examples.
A reference is matched against the stored ones before a new row is created.
A reference whose example is never stored must create nothing.

## Inline notes

### `public void MarkupImport_ReferenceWithoutTitle_CreatesOwnReference()`

A stored reference with no title and no year must not catch every file reference that has none either.

### `public void MarkupImport_TextlessSentenceReference_CreatesNoReference()`

The sentence keeps its particle and dependence, but its example has no text and is never stored.
Its reference carries a new title, so creating it early would leave an orphan behind.

### `public void MarkupImport_MergeTwiceBareReference_KeepsOneReference()`

A reference with no title and no address never matches, so each creation adds a row.
The second Merge drops the sentence as already present, so it must create nothing.
