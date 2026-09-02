# PCardImage.cs

## `internal sealed partial class PCard`

The Image rows a card carries.
Unlike Example and Situation, a card starts with no Image row at all.
It keeps none it is not given.
A picture is an addition the user asks for through the card's Extra row.
So an empty card shows no picture field.
A card whose last picture is dropped goes back to showing none.

## `internal void PCardImageShow(IReadOnlyList<LStateValue> locations)`

Replaces the rows with the stored Images of the card being loaded, one row per location.
It leaves the card with no rows when it references none.

## `internal IReadOnlyList<LStateValue> PCardImageRead()`

What the card says its Images are.
A row whose location stands empty is left out, so a row opened and never filled is written nowhere.

## `internal void PCardImageAdd()`

Opens an empty picture row at the end, which is what the card's Extra row asks for.

## `internal void PCardImageRemove(PImage row)`

Drops the row outright.
There is no last row to keep: a card carrying no picture is an ordinary card.
