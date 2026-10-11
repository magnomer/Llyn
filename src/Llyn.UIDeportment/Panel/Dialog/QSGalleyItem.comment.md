# QSGalleyItem.cs
Hash: `e703bd752e50739a`

## `internal sealed class QSGalleyItem : INotifyPropertyChanged`

One paper sheet of the print canvas, drawn or still a placeholder.
Every sheet exists from the start, so the scroll length is right before any page is drawn.
It raises a change for its picture and shade, so the list fills its container again.

## `internal QSGalleyItem(int index, double width, double height)`

A placeholder sheet for page `index` at the size the draw answered, with no picture and no shade.

## `public event PropertyChangedEventHandler? PropertyChanged;`

Raised only on a real change, so an equal picture or shade refills nothing.

## `public int QSGalleyItemIndex`

The page this sheet shows, counted from zero like the proof and the gate.

## `public string QSGalleyItemNumber`

The sheet's page number, counted from one as a reader counts.

## `public double QSGalleyItemWidth`

The page width in device-independent pixels, as the draw answered it.
The canvas multiplies it by its zoom, so the item never changes with the zoom.

## `public double QSGalleyItemHeight`

The page height in device-independent pixels, as the draw answered it.

## `public ImageSource? QSGalleyItemPicture`

The drawn page as shown, gray when the window shows gray, or null for a placeholder.
The canvas sets it back to null when its cache drops the page.

## `public bool QSGalleyItemShaded`

True when the page is outside the printed range, so the sheet shows its shade and badge.
