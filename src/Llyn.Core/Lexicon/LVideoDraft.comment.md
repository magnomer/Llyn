# LVideoDraft.cs

## `public sealed record LVideoDraft(`

One video row a card is holding before it is stored.
A card names a video by location and by the span of it worth watching, and both are the user's writing.
So the two travel together from the form to the store rather than the location travelling alone.

The span is text and not a pair of moments.
Half-typed text is what a form actually holds, and refusing to carry it would lose what was typed.
Reading it as moments is the player's work, done where a span that says nothing simply plays the whole film.

**Parameters**

- `LVideoDraftLocation` — Where the film is read from, a file path or a web address.
- `LVideoDraftSpan` — The stretch of the film worth watching, written as `mm:ss - mm:ss`.
  An unspecified span is a film watched whole.

## Inline notes

### `public static LVideoDraft LVideoDraftCreate(LStateValue location)`

A row that names a location and nothing else.
Every reader that knew only locations still means exactly that.

### `public bool LVideoDraftEmpty`

A row is empty when it names no film.
A span alone points at nothing, so it never keeps a row alive.
