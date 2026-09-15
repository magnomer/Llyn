# TMarkupImport.cs

## `public sealed class TMarkupImport`

Covers the markup import from a `.llx` file to stored rows under the New intake.
The Merge and Replace intakes are covered by `TMarkupReplace`.
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

### `private static IReadOnlyList<LMarkupIntake> TMarkupIntakeCreate(int count)`

One New intake per entry, in file order.
