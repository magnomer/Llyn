# TInterfaceMarkup.cs

## `internal static partial class TInterface`

The relay for the markup reader and writer.
A test hands text in and reads entries and omissions back through here alone.
An intake for the import is built here too.
A test names an index and a mode and nothing more.
An entry is built here as well, for a writer test that starts from a record rather than text.

## `internal static long TMarkupCeilingRead()`

The largest markup file the file adapter reads, so the oversize test builds one byte past it.

## `internal const string TMarkupPair`

Two entries that each translate to the other, so both links resolve only through each other.

## `internal const string TMarkupLone`

One entry whose translation names an entry the file does not carry.

## `internal static string TMarkupSave(TWorkspace workspace, string text)`

The file lands inside the test workspace, so it is cleaned up with it.

## `internal static LCardDraft TCardCreate(string definition, int position)`

A card with only a definition, enough for a sense that just needs to exist.
The import, export and link tests all build their entries from it.
