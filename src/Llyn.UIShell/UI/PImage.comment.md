# PImage.cs

## `internal sealed class PImage`

One Image row on a card.
The row carries where the picture is read from, and the preview drawn from it.
The location field takes a file on this machine and an address on the web alike.
Both are places a picture lives, and neither is more the picture than the other.

A location standing empty is not one thing.
It may never have been written.
Or it may have been written and be unknown now.
The second is marked rather than shown, and the converter reads the mark off the engine's value.
The row holds that value and nothing typed, so what is typed leaves through the editor as written text.

A preview is not attempted until the row has been seen.
A card holds every picture of every meaning, and most of them sit below the fold when the entry opens.
Decoding a picture nobody has scrolled to would cost the open for nothing.
The row therefore waits for its element to report itself in view, and loads once, then keeps up with edits.

A preview is attempted and allowed to fail.
A path that names nothing and an address that answers nothing leave the row with no preview.
So does a file that is not a picture.
The location the user wrote is still standing.
The row says what was meant, not what could be reached.


An empty row nothing has been written in, which is what the card's Extra row opens.

## `internal PImage(LImageDraft written)`

The row for a stored Image: the location as the store knows it, and the row it stands for.
The row id is carried through untouched, so an edited location updates a picture rather than replacing it.

## `internal long PImageId`

The id of the engine's row this one shows, which every request about it names.

## `public void PImageLoad()`

The element drawing the row has come into view, so the preview is loaded now.
The first call loads and marks the row seen, and later edits to the location reload at once.
A second call does nothing, since a seen row already keeps its preview current.

## `public LStateValue PImageLocation`

The location as the draft holds it, set only from the draft.
A changed location reloads the preview of a row already seen.

## `internal void PImageShow(LImageDraft written)`

Redraws the row from the engine's row where the location differs.
The id is always taken.

## `internal static string? PImageOpen(Window owner)`

Asks the user for a picture file on this machine and answers its path, or null when they chose none.
It lives on the row because every editor drawing the row offers the same chooser.

## `internal static void PImageAttach(LEngine engine)`

Holds the engine every row asks about a location.
A row is built by a converter with no engine at hand, so the window hands it over once.

## `internal static Uri? PImageAddressRead(string location)`

Turns what was typed into an address to load from, or `null` when it names no reachable place.
The engine settles what may be reached, so a relative path means the same thing here as on import.
A local file passes only when it exists.
The Video row and the Screen read their own location the same way, so this is shared, not written twice.
