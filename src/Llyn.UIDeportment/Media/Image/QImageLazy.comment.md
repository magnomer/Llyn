# QImageLazy.cs
Hash: `5ec419d0f109cfbe`

## `public sealed class QImageLazy : Decorator`

The element that tells a picture row when it has come into view.
It wraps the element that draws the picture and holds the row that owns it.
A reading view lays out every card at once, and the scroll region clips what is below the fold.
The framework still measures and draws what is clipped, so an image bound there would decode unseen.
The decorator instead watches the scroll regions above it.
It asks the row to load only once the element is near the window.
Near means within a reach of the viewport on every side, so a slow scroll finds the picture already there.
Nothing here decodes a picture, and nothing here holds one.
It is public because the veneer's script picture template names it.

## `private const double QImageLazyReach`

How far outside the viewport, in device-independent pixels, an element still counts as in view.

## `public static readonly DependencyProperty QImageRowProperty`

The row the decorator reports to, set by the fill to the element's own data.

## `public QImageLazy()`

Checks when the element enters the tree and stops watching when it leaves.
Checks again when the element is shown or hidden.

## `public QImagePending? QImageLazyRow`

The row that will load its picture once told.

## `private static void QImageRowRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

A row bound after the element was already in the tree is checked at once.

## `private void QImageContextRefine(object sender, DependencyPropertyChangedEventArgs e)`

A reading view lists a card's [QLeafImage](../../Display/Template/QLeafImage.comment.md) lines.
A line arriving as the element's data is refined here, and any other data is left alone.

## `private void QImageEmptyRefine(QLeafImage image)`

A line Conduct calls empty collapses the element, since there is nothing to show for it.

## `private void QImagePendingRefine(QLeafImage image)`

The line's loading row becomes the element's data instead, so the line fill finds it as the editor's fill does.

## `private void QImageLoadRefine(object sender, RoutedEventArgs e)`

The first check, made once layout has placed the element.

## `private void QImageDropRefine(object sender, RoutedEventArgs e)`

An element leaving the tree lets go of the scroll regions it watched.
A row already loaded keeps its picture, so a return to the tree draws it at once.

## `private void QImageVisibleRefine(object sender, DependencyPropertyChangedEventArgs e)`

A folded section unfolding is the other way an element comes into view, so it is checked like a scroll.

## `private void QImageScrollRefine(object sender, ScrollChangedEventArgs e)`

Every scroll and every change of extent above the element is a chance it came into view.

## `private void QImageSizeRefine(object sender, SizeChangedEventArgs e)`

A scroll region growing taller shows more, so it is checked like a scroll.

## `private void QImageLazyCheck()`

Loads the row and stops watching when the element is in view, or starts watching when it is not.
No row means nothing to do, since a template may bind one later.

## `private bool QImageShownCheck()`

Whether the element is visible and within reach of the viewport of every scroll region above it.
A region not yet measured is passed over, because it cannot say what it shows.
An element not yet connected to a region cannot be placed and counts as unseen.
An element without a size is judged by its top-left point, since it has one before it has content.

## `private IEnumerable<ScrollViewer> QImageViewerScan()`

The scroll regions between the element and the root, nearest first.

## `private void QImageLazyAttach()`

Watches every scroll region above the element, once.

## `private void QImageLazyDetach()`

Stops watching, so a loaded or removed element costs the regions nothing more.
