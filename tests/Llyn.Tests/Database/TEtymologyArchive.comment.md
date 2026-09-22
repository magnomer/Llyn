# TEtymologyArchive.cs

## `public sealed class TEtymologyArchive`

Covers the etymology store: the narrative row with its spans, the ordered source links, and who names an entry.

## `public void EtymologySave_ProseWithSpans_ReadsThemBackInOrder()`

Spans come back sorted by offset whatever order they were saved in.

## `public void EtymologySave_BlankText_ClearsTheRowAndItsSpans()`

Saving nothing drops the row, and its spans go with it.

## `public void EtymologySave_SpanPastTheTextOrNamingNoEntry_Throws()`

A span must fit the text, name an entry, and overlap no other, and a broken one writes nothing.

## `public void EtymonSet_RepeatsSelfAndNothing_StoresTheRestInOrder()`

Repeats, the entry itself and an empty id are skipped, and the rest are numbered from zero.

## `public void EtymonSet_EmptyList_ClearsEveryLink()`

An empty list is how the links are dropped.

## `public void EtymologySourceScan_NamedEntry_ListsBothShapesOnce()`

The entries that name one entry, by link or by span, each listed once.

## `public void EntryDelete_NamedSource_TakesEveryLinkToItAway()`

Deleting an entry takes every link and span naming it away, leaving the narrative standing.

## Inline notes

### `private static long TEtymologyEntryCreate(LEngine engine, string headword)`

One bare stored entry, since only its id matters to a link.
