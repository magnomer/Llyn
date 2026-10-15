# Image

Picture rows for the editor's cards and the repertoire's situations.
It stands apart from Video because a picture is decoded lazily, only once its row has been seen.

## `QImage`

The driver for Image rows in a card and in a situation.
The rows sit in a template, so the card list hands their buttons here rather than to the card.
Each click hands one raw value to one image gate, and the engine finds the card a row belongs to.
Each owner builds its own driver and hands it its own gate.

### `QImageRowRefine`

A new row holds the engine's location, and nothing typed.

### `QImageRowApply`

The row is drawn with the location's text change unsubscribed.
So a redraw from the draft never reaches the gate as typing.

### `QImageLocationObserve`

Only a field the keyboard is in has been typed into.

### `QImageFreshObserve`

A Situation has no card, so the add names a null card, and the draft routes it.
Null is Conduct's word for no card, so no magic id is sent.

### `QImageOpenObserve`

The chosen path goes raw to the file gate, which sends it as the row's location at once.
A cancelled dialog hands null, and the gate does nothing.
The path is written as it stands rather than copied anywhere.
Browsing is a convenience for the local case only, since the field takes a web address as well.

## `QImageItem`

One Image row on a card, carrying where the picture is read from and the preview drawn from it.
A local file and a web address are both places a picture lives.

An empty location is not one thing.
It may never have been written, or it may have been written and be unknown now.
The second is marked rather than shown, and the converter reads the mark off the engine's value.
The row holds that value and nothing typed, so typed text leaves through the editor as written.

A preview is not attempted until the row has been seen.
Most pictures sit below the fold when the entry opens, and decoding them would cost the open for nothing.
The row loads once on its first sight, then keeps up with edits.

A preview is attempted and allowed to fail.
A path or address that reaches nothing leaves the row with no preview.
So does a file that is not a picture.
The location the user wrote still stands, since the row says what was meant.

### `QImageItemId`

The row id is carried through untouched, so an edited location updates a picture rather than replacing it.
It is fixed at construction, so showing a draft never rebinds it.

### `QImagePendingLoad`

The first call loads and marks the row seen, and a later changed address reloads at once.
A second call does nothing.

### `QImageItemPreview`

Null before the row is seen and when nothing decodes.
A local file is read whole when decoded, so the preview never holds the file open.

### `QImageItemShow`

The id is not taken, since it was fixed at construction.
A changed address reloads the preview of a row already seen, and an unchanged one keeps it.
So a typed location reloads nothing until it reaches a new address.

### `QImageItemOpen`

It lives on the row because every editor drawing the row offers the same chooser.

### `_qImageItemAddress`

The address is ready on the row's state, or null when it reaches nothing.
The engine settles what may be reached, and a local file passes only when it exists.
So the row touches no disk to decide, and a relative path means the same as on import.
It is also the paint memory that tells whether the preview must reload.

### `QImageItemRefine`

The shared fill of a written picture row, used by the entry and situation editors alike.
It attaches the list's reveal, so the remove handle shows while the list is hovered or focused.

### `QImageItemApply`

The fill of a read-only picture line.
It watches the picture, since the preview arrives only once the loader decodes it.

## `QImageLazy`

The element that tells a picture row when it has come into view.
A reading view lays out every card at once, and the framework still draws what the scroll region clips.
An image bound there would decode unseen.
So the decorator watches the scroll regions above it instead.
It asks the row to load only once the element is within a reach of the viewport on every side.
So a slow scroll finds the picture already there.
Nothing here decodes or holds a picture.
It is public because the veneer's script picture template names it.

### `QImageContextRefine`

A line arriving as the element's data is refined here, and any other data is left alone.
A line Conduct calls empty collapses the element.
Otherwise the line's loading row becomes the element's data, so the line fill finds it as the editor's fill does.

### `QImageDropRefine`

An element leaving the tree lets go of the scroll regions it watched.
A row already loaded keeps its picture.

### `QImageVisibleRefine`

A folded section unfolding is another way an element comes into view, so it is checked like a scroll.

### `QImageLazyCheck`

No row means nothing to do, since a template may bind one later.

### `QImageShownCheck`

A region not yet measured is passed over, because it cannot say what it shows.
An element not yet connected to a region counts as unseen.
An element without a size is judged by its top-left point.

## `QImagePending`

What a row holding a picture promises the element that draws it.
The card image row and the script picture both keep this promise.
So one lazy element serves every picture the reading view draws.
It is public because the public lazy element holds a row of this type.
A row already loaded does nothing on a repeat load.

## Stamp

Commit: `2f9a06b8eb95`
- `QImage.cs` `55e72165eb5d301b`
- `QImageItem.cs` `ccbbde9b396cd359`
- `QImageLazy.cs` `1ff9279b6097bd60`
- `QImagePending.cs` `0cdc26e686b37582`
