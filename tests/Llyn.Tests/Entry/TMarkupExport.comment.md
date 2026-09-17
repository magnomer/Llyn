# TMarkupExport.cs

## `public sealed class TMarkupExport`

Covers the markup export from stored rows to a `.llx` file.
Every id a row carries must leave the file as a name, a path or nothing at all.
The portrait export in markup format is the same file by another door.

## Inline notes

### `public void MarkupExport_LinkedEntry_WritesNamesNotIds()`

The entry links to two other entries, a reference with two authors and a sub-sense of the mentioned entry.
The file is parsed back through the reader, so every assertion is on records and not on text.
The one text assertion is that no `id` appears anywhere.

### `public async Task MarkupExport_PortraitMarkupFormat_WritesSameText()`

The label is passed but never read, since markup carries states rather than display words.

### `public void MarkupExport_Exemplar_CarriesEveryText()`

Every string the exemplar draft carries must reach the file, since a person shares what they typed.
A dropped field names itself by its own distinct string.

### `public void MarkupExport_Exemplar_SharedWorkspaceWritesSameText()`

One person exports, another imports into an empty workspace and exports again.
The two files must match, so the loader, writer, reader and importer carry the same fields.
The translation and mention targets travel in the same file, so every name resolves on arrival.

### `public void MarkupExport_Exemplar_SharedWorkspaceLoadsSameDraft()`

The same trip, judged on the stored drafts rather than the files.
Two files can match while both lack a field, so the arriving draft is compared with the sent one.
Ids are zeroed first.
A field the writer never writes, or the importer never reads, breaks the shape.
