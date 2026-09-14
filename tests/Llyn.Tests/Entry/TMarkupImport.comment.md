# TMarkupImport.cs

## `public sealed class TMarkupImport`

Covers the markup import from a `.llx` file to stored rows, in every `LMarkupMode`.
Every link the file names by headword must land on an id or be reported as an omission.
A refusal anywhere in the file must leave the workspace as it was.

## Inline notes

### `public void MarkupImport_TwoEntriesLinked_StoresBothWithLinks()`

Each entry translates to the other, so both links can only resolve through the prepared bare entries.

### `public void MarkupImport_TwinTargets_DropsLinkAndReportsOmission()`

The two stored twins differ only in case, so normalization is what makes them twins.

### `public void MarkupImport_BlankSecondHeadword_StoresNeither()`

The first entry is sound on its own, and only the rollback removes it.

### `public void MarkupImport_ExportedFile_RoundTrips()`

The exported entry is deleted before the file comes back, so the import creates rather than finds it.
The mentioned entry and the reference stay, so the file's names resolve to the rows that were there.
The reference count stays at one, which shows the file's reference was matched and not created again.

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

### `private static string TMarkupSave(TWorkspace workspace, string text)`

The file lands inside the test workspace, so it is cleaned up with it.

### `private static IReadOnlyList<LMarkupIntake> TMarkupIntakeCreate(int count)`

One New intake per entry, in file order.
