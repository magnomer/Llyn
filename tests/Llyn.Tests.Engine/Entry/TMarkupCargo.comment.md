# TMarkupCargo.cs
Hash: `026cdcc245dabbfc`

## `public sealed class TMarkupCargo`

Covers the cargo one read of a `.llx` file hands to the import.
Also what the import refuses of the intakes it is given.
A file past the size ceiling is refused before a byte is read.
A language that could not name a pack folder is blanked at the read and reported at its entry's line.
Two intakes on one target are refused, since the second write would erase the first.
What the read skipped rides in the cargo and comes out of the import unchanged.
The whole import report stops at the omission ceiling and ends with one `...` row.

## Inline notes

### `public void MarkupRead_OversizeFile_RefusesMarkup()`

The file is grown by length alone and holds no content, so the refusal must come from the length check.

### `public void MarkupRead_TraversalLanguage_BlanksAndReportsOmission()`

Both entries carry the same bad name, so both are reported and both are blanked.

### `public void MarkupImport_DuplicateTarget_RefusesItem()`

One Replace and one Merge on the same entry, so the refusal is about the target and not the mode.

### `public void MarkupImport_OmissionsPastCeiling_EndsWithOneEllipsis(int origins, int etymons)`

Each unknown `origin` is a read row and each unlinked `etymon` an import row.
The first case passes the read cap too, so the two caps must not leave two `...` rows.
The second case stays under the read cap, so only the outcome clamp can stop the list.
