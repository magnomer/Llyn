# TMarkupReplace.cs
Hash: `92ea41fb2567f516`

## `public sealed class TMarkupReplace`

Covers the markup import under the Replace intake, which lands on a stored entry.
Replace keeps the entry id and what hangs off it, and rebuilds the rest from the file.
A refusal on one target rolls back every entry in the file.

## Inline notes

### `public void MarkupImport_ReplaceEntry_KeepsIdGraspFavoriteAndRenames()`

Grasp and favorite hang off the entry id, so a Replace that keeps the id keeps them.

### `public void MarkupImport_ReplaceCitedSense_ClearsMentionSenseAndReportsOmission()`

The other entry's example cites a sense the Replace deletes, so the mention keeps only the entry.

### `public void MarkupImport_ReplaceCitedSense_ReportsLineAndText()`

The detach runs before any other row of the entry, and its row must still take the entry's line.
The exact text proves the example is named by its sentence and not by its id.

### `public void MarkupImport_ReplaceEntry_KeepsSituationAndDetachedExample()`

The situation and the example lose their card but stay stored, since detaching never deletes.

### `public void MarkupImport_HeldDraftOnTarget_RefusesWholeFile()`

The New intake comes first in the file, yet the stale target refuses before any entry is created.
