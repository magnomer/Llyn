# QImage.cs
Hash: `9de6b54bb350ecb0`

## `internal sealed class QImage`

The driver for Image rows, in a card of the editor and in a situation of the repertoire.
It hears a row opened, a row dropped, and a file chosen from this machine.
The rows sit in a template, so the card list hands their buttons here rather than to the card.
Each click hands one raw value to one image gate, and the engine finds the card a row belongs to.

## `internal void QImageIntroduce(CImage image)`

Holds the image gate the row clicks reach, the editor's or the repertoire's.
Each owner builds its own driver and hands it its own gate.

## `internal void QImageAddObserve(object sender, RoutedEventArgs e)`

Hands the id of the card the Extra row belongs to to the add gate, which appends the row.

## `private void QImageRemoveObserve(object sender, RoutedEventArgs e)`

Hands the id of the clicked row to the remove gate.

## `private void QImageOpenObserve(object sender, RoutedEventArgs e)`

Asks for a picture file, owned by the window of the clicked button.
The chosen path goes raw to the file gate, which sends it as the row's location at once.
A cancelled dialog hands null, and the gate does nothing.
A typed location goes the same way through the editor's field handler, only deferred.
Browsing is a convenience for the local case only.
The field takes a web address just as well.
The path chosen here is written as it stands rather than copied anywhere.

## `internal void QImageApply(FrameworkElement container, object item, string? name)`

The picture row fill, reached from the card templates' list forwarders and the situation editor's fill.
It draws the row through the shared fill and subscribes the browse and remove observers.

## `internal void QImageFieldObserve(QImageItem row, string text)`

Hands a typed location to the location gate, heard through the editor's field handler or the situation's typing handler.
