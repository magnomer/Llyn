# QImageItem.cs
Hash: `9c1a26bf24f875c1`

## `internal sealed class QImageItem : INotifyPropertyChanged, QImagePending`

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

## `internal QImageItem(CImageDraft written)`

The row for a stored Image, holding the location as the store knows it and the row it stands for.
It also holds the ready address its preview is read from.
The row id is carried through untouched, so an edited location updates a picture rather than replacing it.

## `internal long QImageItemId`

The id of the engine's row this one shows, which every request about it names.

## `public void QImagePendingLoad()`

The element drawing the row has come into view, so the preview is loaded now.
The first call loads and marks the row seen, and a later changed address reloads at once.
A second call does nothing, since a seen row already keeps its preview current.

## `public CStateWording QImageItemLocation`

The location as the draft holds it, set only from the draft.

## `public ImageSource? QImageItemPreview`

Null before the row is seen and when nothing decodes, so a null says only that nothing is drawn.
A local file is read whole when decoded, so the preview never holds the file open.

## `internal void QImageItemShow(CImageDraft written)`

Redraws the row from the engine's row where the location differs.
The id is always taken.
A changed address reloads the preview of a row already seen, and an unchanged one keeps it.
So a location typed into the field reloads nothing until it reaches a new address.

## `internal static string? QImageItemOpen(Window owner)`

Asks the user for a picture file on this machine and answers its path, or null when they chose none.
It lives on the row because every editor drawing the row offers the same chooser.

## `private Uri? _qImageItemAddress;`

The address the preview is read from, ready on the row's state, or null when it reaches nothing.
The engine settles what may be reached, so a relative path means the same thing here as on import.
A local file passes only when it exists, which the engine checks, so the row touches no disk to decide.
It is also the paint memory that tells whether the preview must reload.

## `internal static void QImageItemRefine(FrameworkElement container, object item, string? _)`

The shared fill of a written picture row, used by the entry and situation editors alike.
It hands the row to its lazy loader and shows the preview once there is one.
The location text and its hint go through the state converter, with the localized texts read at fill time.
It also attaches the list's reveal, so the remove handle shows while the list is hovered or focused.

## `internal static void QImageItemApply(FrameworkElement container, object item, string? _)`

The fill of a read-only picture line, whose row is the picture the lazy loader made.
It watches that picture, since the preview arrives only once the loader decodes it.

## `private void QImageItemBuild()`

Decodes the preview from the ready address once the row has been seen.
A missing address, or one that does not decode, leaves the row with no preview.
