# TEasel.cs
Hash: `f85c1bca81bff00c`

## `public sealed class TEasel`

The easel's media edits, one fact per member, each over a fresh situation tenure.
The child card facts hold a fresh entry instead, since only an entry has cards.

## `private const int TEaselHold = 600000;`

A wait no test outlives, so a deferred edit stays queued until the fact flushes it by hand.

## `public void ImageAdd_FreshSituation_AddsEmptyRow()`

An added picture row starts with no location.

## `public void ImageRemove_AddedRow_DropsIt()`

An added picture row is dropped by its id.

## `public void ImageRemove_ChildCard_DropsIt()`

A picture row on a child card is dropped by its id, since the card find walks the children.

## `public void ImageSet_Deferred_WritesOnlyAfterFlush()`

A typed location waits in the queue and lands on the flush.

## `public void VideoAdd_FreshSituation_AddsEmptyRow()`

An added video row starts with no location.

## `public void VideoRemove_AddedRow_DropsIt()`

An added video row is dropped by its id.

## `public void VideoRemove_ChildCard_DropsIt()`

A video row on a child card is dropped by its id, as the picture row is.

## `public void VideoSet_ChosenFile_WritesAtOnce()`

A chosen file lands at once, even under a long delay.

## `public void SpanSet_Typed_WritesSpan()`

A typed timestamp lands on the video row.

## `private static LTenure TEaselStart(LEngine engine, int delay)`

Sets the delay and starts a tenure on a fresh situation.
