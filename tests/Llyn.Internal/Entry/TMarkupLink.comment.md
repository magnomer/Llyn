# TMarkupLink.cs

## `public sealed class TMarkupLink`

Covers the links a markup import makes by name, the sense pass above all.
A sense cited by position path must land on the sense that exists once the file is stored.
A span the store cannot take must be dropped with an omission, never fail the file.

## Inline notes

### `public void MarkupImport_ReplaceOwnSense_PointsMentionAtNewSense()`

The file cites a sense of the entry it replaces.
The old sense id is gone by the time the mention is stored.
Before the sense pass this failed the whole import on a foreign key.

### `public void MarkupImport_TwoNewEntriesCitingEachOther_LinksBothSenses()`

Each entry cites a sense of the other, so neither sense exists when the citing entry is written.

### `public void MarkupImport_SenseBeyondTree_KeepsEntryAndReportsLine()`

The path names a fourth meaning of an entry with one, so the mention keeps only the entry.
The omission carries the entry's line, since the record has none of its own.

### `public void MarkupImport_MentionPastText_DropsMentionAndReports()`

The span runs past the example text, which the archive would refuse with an exception.

### `public void MarkupImport_MentionWithoutHeadword_KeepsSpanAlone()`

A span that stands for no entry is stored, and the export writes it back with no headword.

### `public void MarkupImport_ReplaceWithoutLanguage_KeepsStoredLanguage()`

A Replace file with no language element must not blank the stored language.

### `public void MarkupImport_MentionOffsetOverflow_DropsMention()`

An offset of `int.MaxValue` plus its length wraps negative in `int` and would pass the end check.

### `public void MarkupImport_UncLocations_DropsRowsAndBlanksAudio()`

A UNC path and a `file` address with a host both name a machine Windows would send credentials to.
The web image between them passes, which shows only the refused rows are dropped.

### `public void MarkupImport_ReferenceWithoutTitle_CreatesOwnReference()`

A stored reference with no title and no year must not catch every file reference that has none either.

### `private static LExampleDraft TExampleRead(LCardDraft card)`

The one example a card of these files carries.

### `private static string TMarkupSave(TWorkspace workspace, string text)`

The file lands inside the test workspace, so it is cleaned up with it.
