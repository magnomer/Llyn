# QEasel.cs

## `public sealed class QEasel`

The gate for the image and video rows of one desk.
A driver hands it ids and values, and the gate builds each request inside.
It is sealed, so its public members name only .NET types and Conduct shapes.
It takes the `Q` prefix until the gates move to Conduct, where it becomes `CEasel`.

## `internal QEasel(LDesk desk)`

Only the desk builds one, over itself.

## `public void QEaselImageAdd(long card, int position)`

Sends an empty picture row at the position, on the card, or on the draft when the card is zero.

## `public void QEaselImageRemove(long card, long image)`

Sends the removal of one picture row.

## `public void QEaselImageSet(long image, string location, bool deferred)`

Sets where a picture lives.
Typing defers it, and a chosen file sends it at once.

## `public void QEaselVideoAdd(long card, int position)`

Sends an empty video row at the position, on the card, or on the draft when the card is zero.

## `public void QEaselVideoRemove(long card, long video)`

Sends the removal of one video row.

## `public void QEaselVideoSet(long video, string location, bool deferred)`

Sets where a video lives.
Typing defers it, and a chosen file sends it at once.

## `public void QEaselSpanSet(long video, string span)`

Defers the timestamp a video plays from, since only typing sets it.

## `private void QEaselRequestRun(LRequest request, bool deferred)`

Defers or sends the built request, as the caller's parameter says.
