# LEtymologyArchive.cs

## `public sealed class LEtymologyArchive : LEtymologyVault`

The workspace-database adapter for `LEtymologyVault`.
It keeps the narrative in `etymology` with its spans in `etymology_mention`, and the links in `etymon`.
It never decides which of the two shapes an entry keeps, which the clerk decides.

## `public LEtymology? LEtymologyRead(long entryId)`

Reads the entry's narrative row and its spans, or null when the entry has no row.

## `public IReadOnlyList<LEtymon> LEtymologyEtymonRead(long entryId)`

Reads the entry's direct links in position order.

## `public LEtymology? LEtymologySave(long entryId, LEtymology? etymology)`

Writes the whole narrative of the entry in one transaction.
A null or blank narrative deletes the row, and its spans go with it by cascade.
The old row is deleted before the new one is written, so span ids are minted fresh.
The spans are checked against the text before anything is written.

## `public IReadOnlyList<LEtymon> LEtymologyEtymonSet(long entryId, IReadOnlyList<long> targetIds)`

Rewrites the entry's links as the ids given, numbering the positions from zero.
A zero, a repeat and the entry itself are skipped rather than refused.

## `public IReadOnlyList<LEntry> LEtymologySourceScan(long entryId)`

The entries that name this one, whether through a link or through a span of their prose.
One entry is listed once even when it names the target both ways.

## `private static IReadOnlyList<LMention> LEtymologyMentionRead(SqliteConnection connection, long etymologyId)`

Reads the spans of one narrative in offset order.

## `private static long LEtymologyMentionSave(SqliteConnection connection, long etymologyId, LMention mention)`

Writes one span under its narrative and returns the id it was given.

## `private static void LEtymologyClear(SqliteConnection connection, long entryId)`

Deletes the entry's narrative row, and its spans with it.

## `private static void LEtymologyValidate(string text, IReadOnlyList<LMention> mentions, long entryId)`

Refuses a span that names no Entry, falls outside the text, or overlaps another span.
The text is measured in code points, as the offsets are.
