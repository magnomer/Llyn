# PImage.cs

## `internal sealed class PImage`

One Image row on a card.
The row carries where the picture is read from, and the preview drawn from it.
The location field takes a file on this machine and an address on the web alike.
Both are places a picture lives, and neither is more the picture than the other.

A location standing empty is not one thing.
It may never have been written.
Or it may have been written and be unreadable now.
The second is marked rather than shown.
The mark stands until the user writes over it or the row is dropped.
Writing in the row is the user saying what the location is, which is why any edit ends the mark.

A preview is attempted and allowed to fail.
A path that names nothing and an address that answers nothing leave the row with no preview.
So does a file that is not a picture.
The location the user wrote is still standing.
The row says what was meant, not what could be reached.

## `internal PImage()`

An empty row nothing has been written in, which is what the card's Extra row opens.

## `internal PImage(LImageDraft written)`

The row for a stored Image: the location as the store knows it, and the row it stands for.
The row id is carried through untouched, so an edited location updates a picture rather than replacing it.

## `internal void PImageIdentityApply(LImageDraft stored)`

Takes the id the engine gave this row when it saved the draft.
The row never mints an id of its own.

## `internal LImageDraft PImageDraftRead()`

What the row says its Image is: nothing written, unreadable, or the text it shows.
The stored row it came from travels back with it, empty for a row the user opened.

## `internal static Uri? PImageAddressRead(string location)`

Turns what was typed into an address to load from, or `null` when it names no reachable place.
An absolute address is taken as written, so `https://` and a drive path both pass.
Anything else is treated as a path relative to where the program runs.
It passes only when that file exists.
The Video row reads its own location the same way, which is why this is shared rather than written twice.
