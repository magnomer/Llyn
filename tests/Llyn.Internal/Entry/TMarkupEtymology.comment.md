# TMarkupEtymology.cs

## `public sealed class TMarkupEtymology`

Covers the etymology crossing a markup file: what the writer emits, what the reader resolves, and the round trip.

## `public void MarkupExport_NarratedEtymology_WritesNamesNotIds()`

The narrative travels with its spans, each naming its entry by headword and language and no sense.

## `public void MarkupExport_LinkedEtymology_WritesOneEtymonPerSourceInOrder()`

Only the stored shape is written, the links in stored order.

## `public void MarkupImport_BothShapes_KeepsTheNarrativeAlone()`

A file may carry both shapes, and the engine stores the narrative.

## `public void MarkupImport_EtymonNamingNoEntry_ReportsAnOmission()`

A link the workspace cannot resolve is dropped and named.

## `public void MarkupImport_SpanOutOfRangeOrNamingNoEntry_ReportsAnOmission()`

A span past the text, a span naming no entry, and a sense on a span are each reported.

## `public void MarkupImport_MergeOverAStoredEtymology_LeavesTheStoredOneAlone()`

Merge never overwrites what the entry already says about its origin.

## `public void MarkupExport_NarratedEtymology_ImportsBackTheSameWay()`

An exported entry read back as new carries the same text and the same span.

## Inline notes

### `private static LMarkupEntry TEtymologyExport(LEngine engine, TWorkspace workspace, long entryId, out string text)`

Writes the entry to a file and hands back the parsed record beside the text.
A test may then assert on either.

### `private static LEntry TEtymologyImport(LEngine engine, TWorkspace workspace, string body, out IReadOnlyList<LMarkupOmission> omissions)`

Wraps one entry body in a file and imports it as new.

### `private static void TEtymologyProseSave(LEngine engine, long entryId, string text, int offset, int length, long source)`

Stores a narrative etymology with one span through the request flow.

### `private static void TEtymologySourceSave(LEngine engine, long entryId, IReadOnlyList<long> sources)`

Stores the linked shape, each source appended after the last.

### `private static LEtymologyDraft TEtymologyRead(LEngine engine, long entryId)`

The etymology as the store gives it back.

### `private static long TEtymologyEntryCreate(LEngine engine, string headword)`

One bare stored entry, since only its id matters to a link.
