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
