# TEasel.cs

## `public sealed class TEasel`

The easel's media edits, one fact per member, each over a fresh situation tenure.

## `private const int TEaselHold = 600000;`

A wait no test outlives, so a deferred edit stays queued until the fact flushes it by hand.

## `public void ImageAdd_FreshSituation_AddsEmptyRow()`

An added picture row starts with no location.

## `public void ImageRemove_AddedRow_DropsIt()`

An added picture row is dropped by its id.

## `public void ImageSet_Deferred_WritesOnlyAfterFlush()`

A typed location waits in the queue and lands on the flush.

## `public void VideoAdd_FreshSituation_AddsEmptyRow()`

An added video row starts with no location.

## `public void VideoRemove_AddedRow_DropsIt()`

An added video row is dropped by its id.

## `public void VideoSet_ChosenFile_WritesAtOnce()`

A chosen file lands at once, even under a long delay.

## `public void SpanSet_Typed_WritesSpan()`

A typed timestamp lands on the video row.

## `private static LTenure TEaselStart(LEngine engine, int delay)`

Sets the delay and starts a tenure on a fresh situation.
