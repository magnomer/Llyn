# LEasel.cs
Hash: `d253cf368264ecad`

## `public sealed class LEasel`

The typed image and video edits of one tenure, each building exactly one request.
A driver hands it ids and values, and it sends or defers the request.

## `private readonly LTenure _lEaselTenure;`

The tenure every request is built for and handed to.

## `public LEasel(LTenure tenure)`

Builds the easel over one tenure, which it never swaps.

## `public void LEaselImageAdd(long card)`

Sends an empty picture row after the rows held, on the card or on a zero card's draft.
The end place is read from the held draft, so no caller hands a count.

## `public void LEaselImageRemove(long image)`

Sends the removal of one picture row from the card the draft finds holding it.
A row no card holds sends nothing.

## `public void LEaselImageSet(long image, string location, bool deferred)`

Sets where a picture lives.
Typing defers it, and a chosen file sends it at once.

## `public void LEaselVideoAdd(long card)`

Sends an empty video row after the rows held, as the picture add does.

## `public void LEaselVideoRemove(long video)`

Sends the removal of one video row from the card the draft finds holding it.

## `public void LEaselVideoSet(long video, string location, bool deferred)`

Sets where a video lives.
Typing defers it, and a chosen file sends it at once.

## `public void LEaselSpanSet(long video, string span)`

Defers the timestamp a video plays from, since only typing sets it.

## `private void LEaselRequestRun(LRequest request, bool deferred)`

Defers or sends the built request, as the caller's parameter says.
