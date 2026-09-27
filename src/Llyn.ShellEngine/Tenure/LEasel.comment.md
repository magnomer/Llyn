# LEasel.cs

## `public sealed class LEasel`

The typed image and video edits of one tenure, each building exactly one request.
A driver hands it ids and values, and it sends or defers the request.

## `private readonly LTenure _lEaselTenure;`

The tenure every request is built for and handed to.

## `public LEasel(LTenure tenure)`

Builds the easel over one tenure, which it never swaps.

## `public void LEaselImageAdd(long card, int position)`

Sends an empty picture row at the position, on the card, or on the draft when the card is zero.

## `public void LEaselImageRemove(long card, long image)`

Sends the removal of one picture row.

## `public void LEaselImageSet(long image, string location, bool deferred)`

Sets where a picture lives.
Typing defers it, and a chosen file sends it at once.

## `public void LEaselVideoAdd(long card, int position)`

Sends an empty video row at the position, on the card, or on the draft when the card is zero.

## `public void LEaselVideoRemove(long card, long video)`

Sends the removal of one video row.

## `public void LEaselVideoSet(long video, string location, bool deferred)`

Sets where a video lives.
Typing defers it, and a chosen file sends it at once.

## `public void LEaselSpanSet(long video, string span)`

Defers the timestamp a video plays from, since only typing sets it.

## `private void LEaselRequestRun(LRequest request, bool deferred)`

Defers or sends the built request, as the caller's parameter says.
