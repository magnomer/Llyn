# PCardVideo.cs

## `internal sealed partial class PCard`

The Video rows a card carries.
A Video is kept with the card exactly as an Image is.
A card that is saved and opened again shows the videos it was saved with.

## `internal void PCardVideoShow(IReadOnlyList<LStateValue> locations)`

Fills the rows from a stored card, one row per location, in the order given.

## `internal IReadOnlyList<LStateValue> PCardVideoRead()`

The locations the rows state, skipping every row nothing was written in.

## `internal void PCardVideoAdd()`

Opens an empty video row at the end, which is what the card's Extra row asks for.

## `internal void PCardVideoRemove(PVideo row)`

Drops the row outright.
