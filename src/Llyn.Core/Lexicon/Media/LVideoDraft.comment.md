# LVideoDraft.cs
Hash: `73038fe3640b1b10`

## `public sealed record LVideoDraft(LStateValue LVideoDraftLocation, LStateValue LVideoDraftSpan, long LVideoDraftId = 0)`

One video row a card is holding before it is stored.
A card names a video by location and by the span of it worth watching.
Both are the user's writing.
So the two travel together from the form to the store rather than the location travelling alone.

The span is text and not a pair of moments.
Half-typed text is what a form actually holds, and refusing to carry it would lose what was typed.
`LVideo.LVideoSpanRead` reads it as moments, where a span that says nothing plays the whole film.

**Parameters**

- `LVideoDraftLocation` — Where the film is read from, a file path or a web address.
- `LVideoDraftSpan` — The stretch of the film worth watching, written as `mm:ss - mm:ss`.
  An unspecified span is a film watched whole.
- `LVideoDraftId` — The stored row this draft stands for, zero when the row is new.
  A card points at a video rather than owning one, and two cards may point at the same film.
  Carrying the row id beside the location is what lets a citation stay a citation.

## Inline notes

### `public bool LVideoDraftEmpty`

A row is empty when it names no film.
A span alone points at nothing, so it never keeps a row alive.

## `public LVideoDraft LVideoDraftNormalize()`

The same row with every unreadable value dropped to unspecified.
Called only after the user agreed to lose what the store could not read.
