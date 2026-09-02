# PImage.cs

## `internal sealed class PImage`

One Image row on a card. The row carries where the picture is read from and the preview drawn from it; the location field takes a file on this machine and an address on the web alike, because both are places a picture lives and neither is more the picture than the other.

A location standing empty is not one thing. It may never have been written, or it may have been written and be unreadable now: the second is marked rather than shown, and the mark stands until the user writes over it or the row is dropped. Writing in the row is the user saying what the location is, which is why any edit ends the mark.

A preview is attempted and allowed to fail. A path that names nothing, an address that answers nothing, and a file that is not a picture all leave the row with no preview and the location the user wrote still standing — the row says what was meant, not what could be reached.

## `internal PImage()`

An empty row nothing has been written in, which is what the card's Extra row opens.

## `internal PImage(LStateValue location)`

The row for a stored Image: the location as the store knows it.

## `internal LStateValue PImageLocationRead()`

What the row says its location is: nothing written, unreadable, or the text it shows.

## `internal static Uri? PImageAddressRead(string location)`

Turns what was typed into an address to load from, or `null` when it names no reachable place. An absolute address is taken as written, so `https://` and a drive path both pass; anything else is treated as a path relative to where the program runs, and only when that file exists. The Video row reads its own location the same way, which is why this is shared rather than written twice.
