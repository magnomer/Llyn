# TMarkupReplace.cs

## `public sealed class TMarkupReplace`

Covers the markup import under the Merge and Replace intakes, which land on a stored entry.
Merge joins what the file adds onto the stored cards and keeps every id it can.
Replace keeps the entry id and what hangs off it, and rebuilds the rest from the file.
A refusal on one target rolls back every entry in the file.

## Inline notes

### `public void MarkupImport_AppendKnownMeaning_JoinsCardAndKeepsId()`

The file's first meaning names a stored head, so its tag joins that card under its old id.
The file's second meaning is new and lands third, after the two stored ones.

### `public void MarkupImport_AppendKnownIpa_SkipsRowAndJoinsNote()`

The first file pronunciation equals the stored ipa and is skipped, and the second is new and kept.

### `public void MarkupImport_ReplaceEntry_KeepsIdGraspFavoriteAndRenames()`

Grasp and favorite hang off the entry id, so a Replace that keeps the id keeps them.

### `public void MarkupImport_ReplaceCitedSense_ClearsMentionSenseAndReportsOmission()`

The other entry's example cites a sense the Replace deletes, so the mention keeps only the entry.

### `public void MarkupImport_ReplaceEntry_KeepsSituationAndDetachedExample()`

The situation and the example lose their card but stay stored, since detaching never deletes.

### `public void MarkupImport_HeldDraftOnTarget_RefusesWholeFile()`

The New intake comes first in the file, and the refusal on the second target rolls it back too.
